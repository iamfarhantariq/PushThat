const express = require('express');
const app = express();
app.use(express.json());
const port = 3000;
const projectName = 'PushThat';
var mysql = require('mysql');
var nodemailer = require('nodemailer');
var random = require("randomstring");
var sixDigCode = require('generate-sms-verification-code')
const secret = 'pushnotificationservices';
let jwt = require('jsonwebtoken');
var CronJob = require('cron').CronJob;
var FCM = require('fcm-node');
var serverKey = "<>"; //put your server key here
var fcm = new FCM(serverKey);

const emailRegax = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;

function mysqlConnect() {
    return new Promise(
        resolve => {
            connection = mysql.createConnection({
                host: "localhost",
                user: "root",
                password: "",
                database: "push_over",
                multipleStatements: true,
                timezone: 'utc',
                connectTimeout: 60000
            });
            connection.connect();
            resolve(connection);
        });
}

//================================APIs===================================
app.post('/createAccount', async (req, res) => {
    console.log('createAccount called')
    try {
        var email = req.body.Email;
        var password = req.body.Password;
        var time = new Date();
        var time1 = time.toISOString();
        var timestamp = time1.substr(0, time1.length - 1);

        if (!email || !password || email == '' || password == '')
            return res.send({ Status: false, MessageCode: 'PA116', Message: 'Fill all the fields' })

        if (!emailRegax.test(email))
            return res.send({ Status: false, MessageCode: 'PA101', Message: 'Email is invalid' });

        if (password.lenght < 8 && password.lenght > 16)
            return res.send({ Status: false, MessageCode: 'PA103', Message: 'Password must be between 8 to 16 characters.' });

        var connection = await mysqlConnect();

        connection.query('INSERT into `users` (`email`,`password`,`varified`,`created_at`) VALUES (?)',
            [[email, password, 0, timestamp]], async (err, rows) => {
                if (err) {
                    if (err.errno == 1062) {
                        connection.destroy();
                        return res.send({ Success: false, MessageCode: 'PA102', Message: "Already registered." });
                    } else {
                        connection.destroy();
                        return res.send({ Success: false, MessageCode: 'PA000', Message: err });
                    }
                }
                else {
                    //generate account key for this email and inserted to user_keys table
                    var pushKey = random.generate({ length: 30, charset: 'alphanumeric' });
                    await QueryExecution('INSERT into `users_key` (`users_id`, `push_over_key`) VALUES (?)',
                        [[rows.insertId, pushKey]], connection);

                    //inserting licensing time period for user    
                    time.setDate(time.getDate() + 7);
                    var expired_at = new Date(time).toISOString();
                    await QueryExecution('INSERT into `licensing` (`users_id`, `licensing`, `created_at`, `expired_at`, `expired`) VALUES (?)',
                        [[rows.insertId, 'TRIAL', timestamp, expired_at.substr(0, expired_at.length - 1), 1]], connection);

                    //inserting notification default setting for user
                    await QueryExecution('INSERT into `notification_settings` (`users_id`, `notification_dismissal_sync`) VALUES (?)',
                        [[rows.insertId, 0]], connection);

                    //generate token
                    let token = newToken(pushKey);

                    //inserted code to database for this email.
                    var code = sixDigCode(6, { type: 'number' });
                    sendEmailCode(email, code);
                    await QueryExecution('INSERT into `forget_password` (`users_id`, `code` ,`time`) VALUES (?)',
                        [[rows.insertId, code, timestamp]], connection);

                    connection.destroy();
                    res.send({ Sucess: true, Token: token })
                }
            });
    }
    catch (err) {
        console.log(err);
        res.send({ Success: false, MessageCode: 'PA000', Message: err.message });
    }

});
app.post('/confirmCode', async (req, res) => {
    console.log('confirmEmail is called')
    try {
        var email = req.body.Email;
        var code = req.body.Code;

        if (!email || !code || email == '' || code == '')
            return res.send({ Success: false, MessageCode: 'PA116', Message: 'Fill all the fields' });

        var connection = await mysqlConnect();

        var row = await QueryExecution('SELECT id FROM `users` WHERE email = ?', [email], connection);
        var pushKeyRow = await QueryExecution('SELECT push_over_key FROM `users_key` WHERE users_id = ?', [row[0].id], connection);

        //token varification//
        var token = req.headers['x-access-token'] || req.headers['authorization']; // Express headers are auto converted to lowercase
        if (!token) {
            connection.destroy();
            return res.send({ Success: false, MessageCode: 'PA120', Message: 'No token sent' });
        }
        var obj = await varifyToken(token, pushKeyRow[0].push_over_key);
        if (!obj.Status) {
            connection.destroy();
            return res.status(400).send(obj);
        }

        connection.query('SELECT code, time FROM `forget_password` WHERE users_id= ?', [[row[0].id]],
            async (err, rows, fields) => {
                //console.log(rows);
                if (err) {
                    connection.destroy();
                    return res.send({ Success: false, MessageCode: 'PA000', Message: err });
                }
                else {
                    if (code != rows[0].code) {
                        connection.destroy();
                        return res.send({ Status: false, MessageCode: "PA104", Message: "Incorrect Code" });
                    } else {
                        var codeAddedTime = new Date(rows[0].time);
                        var currentTime = new Date();
                        if (currentTime.getTime() >= codeAddedTime.getTime() + 5 * 60000) {
                            connection.destroy();
                            return res.send({ Status: false, MessageCode: "PA105", Message: "Code Expired" });
                        } else {
                            connection.query('UPDATE `users` SET varified = 1 WHERE id= ?',
                                [[row[0].id]], async (err, result) => {
                                    if (err) {
                                        connection.destroy();
                                        return res.send({ Success: false, MessageCode: 'PA000', Message: err });
                                    }
                                    else {
                                        connection.destroy();
                                        return res.send({ Status: true })
                                    }
                                });
                        }
                    }
                }
            });
    }
    catch (err) {
        console.log(err);
        res.send({ Success: false, MessageCode: 'PA000', Message: err.message });
    }
});
app.post('/login', async (req, res) => {
    console.log('login is called')
    try {
        var email = req.body.Email;
        var password = req.body.Password;

        if (!email || !password || email == '' || password == '')
            return res.send({ Success: false, MessageCode: 'PA116', Message: 'Fill all the fields' });

        if (!emailRegax.test(email))
            return res.send({ Status: false, MessageCode: 'PA101', Message: 'Email is invalid' });

        var connection = await mysqlConnect();
        var userRow = await QueryExecution('SELECT id, email, password FROM `users` WHERE email = ? AND password = ?', [email, password], connection);

        console.log(userRow);

        if (!userRow[0]) {
            connection.destroy();
            return res.send({ Status: false, MessageCode: 'PA106', Message: 'Email or password is incorrect' });
        }

        var pushKeyRow = await QueryExecution('SELECT push_over_key FROM `users_key` WHERE users_id = ?', [userRow[0].id], connection);
        var licensingRow = await QueryExecution('SELECT licensing FROM `licensing` WHERE users_id = ?', [userRow[0].id], connection);
        var token = newToken(pushKeyRow[0].push_over_key);

        connection.destroy();
        res.send({ Success: true, NotificationKey: pushKeyRow[0].push_over_key, Token: token, License: licensingRow[0].licensing });
    }
    catch (err) {
        console.log(err);
        res.send({ Success: false, MessageCode: 'PA000', Message: err.message });
    }
});
app.post('/addDevice', async (req, res) => {
    console.log('add Device is called')
    try {
        var pushKey = req.body.NotificationKey;
        var deviceName = req.body.DeviceName;
        var deviceKey = req.body.DeviceKey;
        var deviceToken = req.body.DeviceToken;
        var platform = req.body.Platform;
        var time = new Date();
        var time1 = time.toISOString();
        var timestamp = time1.substr(0, time1.length - 1);

        if (!pushKey || !deviceName || !deviceKey || !platform || !deviceToken ||
            pushKey == '' || deviceName == '' || deviceKey == '' || deviceToken == '' || platform == '')
            return res.send({ Success: false, MessageCode: 'PA116', Message: 'Fill all the fields' });

        var tokenArr = [];
        tokenArr.push(deviceToken);

        //token varification//
        var token = req.headers['x-access-token'] || req.headers['authorization']; // Express headers are auto converted to lowercase
        if (!token)
            return res.send({ Success: false, MessageCode: 'PA120', Message: 'No token sent' });
        var obj = await varifyToken(token, pushKey);
        if (!obj.Status) {
            return res.status(400).send(obj);
        }

        var connection = await mysqlConnect();

        var pushKeyRow = await QueryExecution('SELECT users_id FROM `users_key` WHERE push_over_key = ?', [pushKey], connection);
        // console.log(pushKeyRow[0].users_id);
        var devicesRow = await QueryExecution('SELECT * FROM `devices` WHERE users_id = ?',
            [pushKeyRow[0].users_id], connection);
        // console.log(devicesRow);
        var licenseRow = await QueryExecution('SELECT * FROM `licensing` WHERE users_id = ?',
            [pushKeyRow[0].users_id], connection);

        var sql = 'INSERT into `devices` (`users_id`, `device_name`, `device_key`, `device_token`, `active`, `created_at`, `last_sync`, `message_delivered_count`, `platform`) VALUES (?)';
        var values = [[pushKeyRow[0].users_id, deviceName, deviceKey, deviceToken, 1, timestamp, timestamp, 0, platform]];
        if (!devicesRow[0]) {
            var licenseBody = `Your device ${deviceName} is associated with ${licenseRow[0].licensing} account.`;
            var deviceStatusBody = `Your device ${deviceName} is ${licenseRow[0].expired == 0 ? 'disabled' : 'enabled'} for push notification services.`;

            connection.query(sql, values, async (err, result) => {
                if (err) {
                    connection.destroy();
                    return res.send({ Success: false, MessageCode: 'PA000', Message: err });
                }
                else {
                    //console.log(result.insertId);
                    var resp = await insertMessageToDB(pushKeyRow[0].users_id, `Welcome to ${projectName}`, `${projectName}`,
                        `Welcome to ${projectName}`, timestamp, 'https://www.pngrepo.com/download/223032/playstore.png', 1, connection);
                    //console.log(resp);

                    var licenseNoti = await sendMessage(tokenArr, 'License Information', licenseBody, `${projectName}`, 'icon.png', timestamp);
                    //console.log(licenseNoti);
                    var statusNoti = await sendMessage(tokenArr, `Your ${deviceName} status`, deviceStatusBody, `${projectName}`, 'icon.png', timestamp);
                    //console.log(statusNoti);
                    connection.destroy();
                    return res.send({ Status: true });
                }
            });
        }
        else {
            var devicesArray = JSON.parse(JSON.stringify(devicesRow));
            var found = devicesArray.find(device => device.device_name === deviceName && device.device_token !== deviceToken);

            if (found === undefined) {
                //console.log('device added');
                var newInserted = await QueryExecution(sql, values, connection);
                if (newInserted.errno == 1062) {
                    var overrided = await QueryExecution('UPDATE `devices` SET device_name = ? WHERE device_token = ?',
                        [deviceName, deviceToken], connection);
                    //console.log(overrided);
                }
                var resp = await insertMessageToDB(pushKeyRow[0].users_id, `Welcome to ${projectName}`, `${projectName}`,
                    `Welcome to ${projectName}`, timestamp, 'https://www.pngrepo.com/download/223032/playstore.png', 1, connection);
                //console.log(resp);

                var licenseNoti = await sendMessage(tokenArr, 'License Information', licenseBody, `${projectName}`, 'icon.png', timestamp);
                //console.log(licenseNoti);

                var statusNoti = await sendMessage(tokenArr, `Your ${deviceName} status`, deviceStatusBody, `${projectName}`, 'icon.png', timestamp);
                //console.log(statusNoti);

                connection.destroy();
                return res.send({ Status: true });
            } else {
                //console.log('device name are same');
                connection.destroy();
                return res.send({ Status: false, MessageCode: 'PA109', Message: 'Device name already exists' });
            }
        }
    }
    catch (err) {
        console.log(err);
        res.send({ Success: false, MessageCode: 'PA000', Message: err.message });
    }
});
app.post('/refreshToken', async (req, res) => {
    console.log('refresh Token is called')
    try {
        var pushKey = req.body.NotificationKey;
        if (!pushKey || pushKey == '')
            return res.send({ Success: false, MessageCode: 'PA116', Message: 'Fill all the fields' });

        var token = req.headers['x-access-token'] || req.headers['authorization']; // Express headers are auto converted to lowercase
        if (!token)
            return res.send({ Success: false, MessageCode: 'PA120', Message: 'No token sent' });

        var tokenNew = newToken(pushKey);

        res.send({ Success: true, Token: tokenNew });
    }
    catch (err) {
        console.log(err);
        res.send({ Success: false, MessageCode: 'PA000', Message: err.message });
    }
});
app.post('/allMessages', async (req, res) => {
    console.log('all Messages is called')
    try {
        var pushKey = req.body.NotificationKey;
        if (!pushKey || pushKey == '')
            return res.send({ Success: false, MessageCode: 'PA116', Message: 'Fill all the fields' });

        //token varification//
        var token = req.headers['x-access-token'] || req.headers['authorization']; // Express headers are auto converted to lowercase
        if (!token)
            return res.send({ Success: false, MessageCode: 'PA120', Message: 'No token sent' });
        var obj = await varifyToken(token, pushKey);
        if (!obj.Status) {
            return res.status(400).send(obj);
        }

        var connection = await mysqlConnect();

        var pushKeyRow = await QueryExecution('SELECT users_id FROM `users_key` WHERE push_over_key = ?', [pushKey], connection);
        // console.log(pushKeyRow[0].users_id);

        var notificationSettingRow = await QueryExecution('SELECT notification_dismissal_sync FROM `notification_settings` WHERE users_id = ?', [pushKeyRow[0].users_id], connection);
        // console.log(notificationSettingRow[0].users_id);

        var messagesRow = await QueryExecution('SELECT * FROM `message` WHERE users_id = ?', [pushKeyRow[0].users_id], connection);
        // console.log(messagesRow);

        if (!messagesRow[0]) {
            connection.destroy();
            return res.send({ Success: true, MessageCode: 'PA113', Message: 'No message found' });
        }

        connection.destroy();
        res.send({
            Success: true, NotificationDismissalSync: notificationSettingRow[0].notification_dismissal_sync == 0 ? false : true,
            Messages: messagesRow
        });
    }
    catch (err) {
        console.log(err);
        res.send({ Success: false, MessageCode: 'PA000', Message: err.message });
    }
});
app.post('/deleteMessage', async (req, res) => {
    console.log('delete Message is called')
    try {
        var pushKey = req.body.NotificationKey;
        var id = req.body.Id;
        if (!pushKey || !id || id == '' || pushKey == '')
            return res.send({ Success: false, MessageCode: 'PA116', Message: 'Fill all the fields' });

        //token varification//
        var token = req.headers['x-access-token'] || req.headers['authorization']; // Express headers are auto converted to lowercase
        if (!token)
            return res.send({ Success: false, MessageCode: 'PA120', Message: 'No token sent' });
        var obj = await varifyToken(token, pushKey);
        if (!obj.Status) {
            return res.status(400).send(obj);
        }

        var connection = await mysqlConnect();
        connection.query('DELETE FROM `message` WHERE id = ?', [[id]], async (err, result) => {
            if (err) {
                connection.destroy();
                return res.send({ Success: false, MessageCode: 'PA000', Message: err });
            }
            else {
                connection.destroy();
                return res.send({ Status: true });
            }
        });
    }
    catch (err) {
        console.log(err);
        res.send({ Success: false, MessageCode: 'PA000', Message: err.message });
    }
});
app.post('/deleteMultipleMessages', async (req, res) => {
    console.log('delete Multiple Messages is called')
    try {
        var pushKey = req.body.NotificationKey;
        var ids = req.body.Ids;
        ids = ids.split(",");
        if (!pushKey || !ids || ids == '' || pushKey == '')
            return res.send({ Success: false, MessageCode: 'PA116', Message: 'Fill all the fields' });

        //token varification//
        var token = req.headers['x-access-token'] || req.headers['authorization']; // Express headers are auto converted to lowercase
        if (!token)
            return res.send({ Success: false, MessageCode: 'PA120', Message: 'No token sent' });
        var obj = await varifyToken(token, pushKey);
        if (!obj.Status) {
            return res.status(400).send(obj);
        }
        var connection = await mysqlConnect();

        ids.forEach(async (element) => {
            var result = await QueryExecution('DELETE FROM `message` WHERE id = ?', element, connection);
            // console.log("Deleted rows :" + result.affectedRows);
        });
        connection.destroy();
        res.send({ Status: true });
    }
    catch (err) {
        console.log(err);
        res.send({ Success: false, MessageCode: 'PA000', Message: err.message });
    }
});
app.post('/deleteAllMessages', async (req, res) => {
    console.log('delete All Messages is called')
    try {
        var pushKey = req.body.NotificationKey;
        if (!pushKey || pushKey == '')
            return res.send({ Success: false, MessageCode: 'PA116', Message: 'Fill all the fields' });

        //token varification//
        var token = req.headers['x-access-token'] || req.headers['authorization']; // Express headers are auto converted to lowercase
        if (!token)
            return res.send({ Success: false, MessageCode: 'PA120', Message: 'No token sent' });
        var obj = await varifyToken(token, pushKey);
        if (!obj.Status) {
            return res.status(400).send(obj);
        }
        var connection = await mysqlConnect();

        var pushKeyRow = await QueryExecution('SELECT users_id FROM `users_key` WHERE push_over_key = ?', [pushKey], connection);
        // console.log(pushKeyRow[0].users_id);

        if (!pushKeyRow[0]) {
            connection.destroy();
            return res.send({ Success: false, MessageCode: 'PA113', Message: 'No message found' });
        }

        connection.query('DELETE FROM `message` WHERE users_id = ?', [[pushKeyRow[0].users_id]], async (err, result) => {
            if (err) {
                connection.destroy();
                return res.send({ Success: false, MessageCode: 'PA000', Message: err });
            }
            else {
                connection.destroy();
                return res.send({ Status: true });
            }
        });
    }
    catch (err) {
        console.log(err);
        res.send({ Success: false, MessageCode: 'PA000', Message: err.message });
    }
});
app.post('/notificationDismissalAsync', async (req, res) => {
    console.log('notification Dismissal Async is called')
    try {
        var pushKey = req.body.NotificationKey;
        var notificationDismissalSync = req.body.NotificationDismissalSync;
        if (!pushKey || !notificationDismissalSync || notificationDismissalSync == '' || pushKey == '')
            return res.send({ Success: false, MessageCode: 'PA116', Message: 'Fill all the fields' });

        //token varification//
        var token = req.headers['x-access-token'] || req.headers['authorization']; // Express headers are auto converted to lowercase
        if (!token)
            return res.send({ Success: false, MessageCode: 'PA120', Message: 'No token sent' });
        var obj = await varifyToken(token, pushKey);
        if (!obj.Status) {
            return res.status(400).send(obj);
        }
        var connection = await mysqlConnect();

        var pushKeyRow = await QueryExecution('SELECT users_id FROM `users_key` WHERE push_over_key = ?', [pushKey], connection);
        // console.log(pushKeyRow[0].users_id);     

        connection.query('UPDATE `notification_settings` SET notification_dismissal_sync = ? WHERE users_id = ?',
            [notificationDismissalSync == true ? 1 : 0, pushKeyRow[0].users_id], async (err, result) => {
                if (err) {
                    connection.destroy();
                    return res.send({ Success: false, MessageCode: 'PA000', Message: err });
                }
                else {
                    connection.destroy();
                    return res.send({ Status: true });
                }
            });
    }
    catch (err) {
        console.log(err);
        res.send({ Success: false, MessageCode: 'PA000', Message: err.message });
    }
})
app.post('/lastsync', async (req, res) => {
    console.log('lastsync is called')
    try {
        var deviceToken = req.body.DeviceToken;
        var time = new Date();
        var time1 = time.toISOString();
        var timestamp = time1.substr(0, time1.length - 1);
        if (!deviceToken || deviceToken == '')
            return res.send({ Success: false, MessageCode: 'PA116', Message: 'Fill all the fields' });

        var connection = await mysqlConnect();
        var deviceRow = await QueryExecution('SELECT users_id FROM `devices` WHERE device_token = ?', [deviceToken], connection);
        // console.log(deviceRow[0].users_id);
        var pushKeyRow = await QueryExecution('SELECT push_over_key FROM `users_key` WHERE users_id = ?', [deviceRow[0].users_id], connection);
        // console.log(pushKeyRow[0].push_over_key);

        //token varification//
        var token = req.headers['x-access-token'] || req.headers['authorization']; // Express headers are auto converted to lowercase
        if (!token) {
            connection.destroy();
            return res.send({ Success: false, MessageCode: 'PA120', Message: 'No token sent' });
        }
        var obj = await varifyToken(token, pushKeyRow[0].push_over_key);
        if (!obj.Status) {
            connection.destroy();
            return res.status(400).send(obj);
        }

        connection.query('UPDATE `devices` SET last_sync = ? WHERE device_token = ?',
            [timestamp, deviceToken], async (err, result) => {
                if (err) {
                    connection.destroy();
                    return res.send({ Success: false, MessageCode: 'PA000', Message: err });
                }
                else {
                    connection.destroy();
                    return res.send({ Status: true });
                }
            });
    }
    catch (err) {
        console.log(err);
        res.send({ Success: false, MessageCode: 'PA000', Message: err.message });
    }
});
app.post('/logout', async (req, res) => {
    console.log('logout is called')
    try {
        var deviceToken = req.body.DeviceToken;
        if (!deviceToken || deviceToken == '')
            return res.send({ Success: false, MessageCode: 'PA116', Message: 'Fill all the fields' });

        var connection = await mysqlConnect();
        var deviceRow = await QueryExecution('SELECT users_id FROM `devices` WHERE device_token = ?', [deviceToken], connection);
        // console.log(deviceRow[0].users_id);

        if (!deviceRow[0]) {
            connection.destroy();
            return res.send({ Success: true });
        }

        var pushKeyRow = await QueryExecution('SELECT push_over_key FROM `users_key` WHERE users_id = ?', [deviceRow[0].users_id], connection);
        // console.log(pushKeyRow[0].push_over_key);

        //token varification//
        var token = req.headers['x-access-token'] || req.headers['authorization']; // Express headers are auto converted to lowercase
        if (!token) {
            connection.destroy();
            return res.send({ Success: false, MessageCode: 'PA120', Message: 'No token sent' });
        }
        var obj = await varifyToken(token, pushKeyRow[0].push_over_key);
        if (!obj.Status) {
            connection.destroy();
            return res.status(400).send(obj);
        }

        connection.query('DELETE FROM `devices` WHERE device_token = ?',
            [[deviceToken]], async (err, result) => {
                if (err) {
                    connection.destroy();
                    return res.send({ Success: false, MessageCode: 'PA000', Message: err });
                }
                else {
                    connection.destroy();
                    return res.send({ Status: true });
                }
            });
    }
    catch (err) {
        console.log(err);
        res.send({ Success: false, MessageCode: 'PA000', Message: err.message });
    }
});
app.post('/forgetPassword', async (req, res) => {
    console.log('forgetPassword called');
    try {
        var email = req.body.Email;
        var time = new Date();
        var time1 = time.toISOString();
        var timestamp = time1.substr(0, time1.length - 1);

        if (!email || email == '')
            return res.send({ Status: false, MessageCode: 'PA116', Message: 'Fill all the fields' })

        if (!emailRegax.test(email))
            return res.send({ Status: false, MessageCode: 'PA101', Message: 'Email is invalid' });

        var connection = await mysqlConnect();

        var userRow = await QueryExecution('SELECT id FROM `users` WHERE email = ?', [email], connection);
        // console.log(userRow[0]);
        if (!userRow[0]) {
            connection.destroy();
            return res.send({ Success: false, MessageCode: 'PA108', Message: "Email not found." });
        }

        //inserted code to database for this email.
        var code = sixDigCode(6, { type: 'number' });
        console.log(code);
        sendEmailCode(email, code);
        var inserted = await QueryExecution('INSERT into `forget_password` (`users_id`, `code` ,`time`) VALUES (?)',
            [[userRow[0].id, code, timestamp]], connection);

        var pushKeyRow = await QueryExecution('SELECT push_over_key FROM `users_key` WHERE users_id = ?', [userRow[0].id], connection);
        // console.log(pushKeyRow[0].push_over_key);

        //generate token
        let token = newToken(pushKeyRow[0].push_over_key);

        connection.destroy();
        res.send({ Sucess: true, Token: token })
    }
    catch (err) {
        console.log(err);
        res.send({ Success: false, MessageCode: 'PA000', Message: err.message });
    }

});
app.post('/newPassword', async (req, res) => {
    console.log('new Password called')
    try {
        var email = req.body.Email;
        var password = req.body.Password;
        var time = new Date();
        var time1 = time.toISOString();
        var timestamp = time1.substr(0, time1.length - 1);

        if (!email || !password || email == '' || password == '')
            return res.send({ Status: false, MessageCode: 'PA116', Message: 'Fill all the fields' });

        if (!emailRegax.test(email))
            return res.send({ Status: false, MessageCode: 'PA101', Message: 'Email is invalid' });

        if (password.lenght < 8 && password.lenght > 16)
            return res.send({ Status: false, MessageCode: 'PA103', Message: 'Password must be between 8 to 16 characters.' });

        var connection = await mysqlConnect();

        var userRow = await QueryExecution('SELECT id FROM `users` WHERE email = ?', [email], connection);
        var pushKeyRow = await QueryExecution('SELECT push_over_key FROM `users_key` WHERE users_id = ?', [userRow[0].id], connection);

        //token varification//
        var token = req.headers['x-access-token'] || req.headers['authorization']; // Express headers are auto converted to lowercase
        if (!token) {
            connection.destroy();
            return res.send({ Success: false, MessageCode: 'PA120', Message: 'No token sent' });
        }
        var obj = await varifyToken(token, pushKeyRow[0].push_over_key);
        if (!obj.Status) {
            connection.destroy();
            return res.status(400).send(obj);
        }

        connection.query('UPDATE `users` SET password = ? WHERE email = ?',
            [password, email], async (err, result) => {
                if (err) {
                    connection.destroy();
                    return res.send({ Success: false, MessageCode: 'PA000', Message: err });
                } else {
                    var tokenRow = await QueryExecution('SELECT device_token FROM `devices` WHERE users_id = ?',
                        [userRow[0].id], connection);

                    if (!tokenRow[0]) {
                        connection.destroy();
                        return res.send({ Success: false, MessageCode: 'PA121', Message: 'No device found' });
                    }
                    var tokens = [];
                    if (tokenRow)
                        for (var i of tokenRow)
                            tokens.push(i.device_token);

                    // console.log(tokens);
                    // console.log(tokens.toString());

                    if (tokens.length >= 1) {
                        var licenseRow = await QueryExecution('SELECT * FROM `licensing` WHERE users_id = ?',
                            [userRow[0].id], connection);
                        var logoutNoti = await sendMessage(tokens, `Password Changed`, `Your password has been reset.`, `${projectName}`, 'icon.png', timestamp, 'Password Reset');
                        // console.log(logoutNoti);

                        var deletedStatus = await QueryExecution('DELETE FROM `devices` WHERE users_id = ?',
                            [[userRow[0].id]], connection);
                        console.log(deletedStatus);

                        connection.destroy();
                        return res.send({ Success: true, NotificationKey: pushKeyRow[0].push_over_key, License: licenseRow[0].licensing });
                    }
                }
            });
    }
    catch (err) {
        console.log(err);
        res.send({ Success: false, MessageCode: 'PA000', Message: err.message });
    }
});
app.post('/pushMessage', async (req, res) => {
    console.log('new Password called')
    try {
        var token = req.body.Tokens;
        token = token.split(",");
        var title = req.body.Title;
        var body = req.body.Body;
        var sender = req.body.Sender;
        var image = req.body.Image;
        var time = new Date();
        var time1 = time.toISOString();
        var timestamp = time1.substr(0, time1.length - 1);

        if (!token || !title || !body || !sender || !image ||
            token == '' || title == '' || body == '' || sender == '' || image == '')
            return res.send({ Status: false, MessageCode: 'PA116', Message: 'Fill all the fields' })

        var tokenArr = [];
        for (var i of token)
            tokenArr.push(i);

        // console.log(tokenArr);
        var notificationStatus = await sendMessage(tokenArr, title, body, sender, image, timestamp);
        // console.log(notificationStatus);

        res.send({ Status: 'Message Sent' });
    }
    catch (err) {
        console.log(err);
        res.send({ Success: false, MessageCode: 'PA000', Message: err.message });
    }
});
app.post('/quitHours', async (req, res) => {
    console.log('quit Hours is called')
    try {
        var pushKey = req.body.NotificationKey;
        var deviceToken = req.body.DeviceToken;
        var checked = req.body.Checked;
        var daysArray = req.body.DaysArray;

        if (!pushKey || !deviceToken || !checked || !daysArray || pushKey == '' || deviceToken == '' || checked == '' || daysArray == '')
            return res.send({ Status: false, MessageCode: 'PA116', Message: 'Fill all the fields' });

        //token varification//
        var token = req.headers['x-access-token'] || req.headers['authorization']; // Express headers are auto converted to lowercase
        if (!token)
            return res.send({ Success: false, MessageCode: 'PA120', Message: 'No token sent' });
        var obj = await varifyToken(token, pushKey);
        if (!obj.Status)
            return res.status(400).send(obj);

        var connection = await mysqlConnect();

        var pushKeyRow = await QueryExecution('SELECT users_id FROM `users_key` WHERE push_over_key = ?', [pushKey], connection);
        // console.log(pushKeyRow[0].users_id);

        for (var arrItem of daysArray) {
            var quitHoursRow = await QueryExecution('SELECT * FROM `quit_hours` WHERE device_token = ? AND days = ?',
                [deviceToken, arrItem.Day], connection);

            if (quitHoursRow.length == 0) {
                connection.query('INSERT into `quit_hours` (`users_id`, `device_token`, `start_time`, `end_time`, `days`, `status`) VALUES (?)',
                    [[pushKeyRow[0].users_id, deviceToken, arrItem.StartTime, arrItem.EndTime, arrItem.Day, checked]], async (err, result) => {
                        if (err) {
                            connection.destroy();
                            return res.send({ Success: false, MessageCode: 'PA000', Message: err });
                        } else {
                            connection.destroy();
                            return res.send({ Success: true });
                        }
                    });
            } else {
                connection.query('UPDATE `quit_hours` SET start_time = ?, end_time = ?, days = ?, status = ? WHERE id = ?',
                    [arrItem.StartTime, arrItem.EndTime, arrItem.Day, checked, quitHoursRow[0].id], async (err, result) => {
                        if (err) {
                            connection.destroy();
                            return res.send({ Success: false, MessageCode: 'PA000', Message: err });
                        } else {
                            connection.destroy();
                            return res.send({ Success: true });
                        }
                    });
            }
        }
    }
    catch (err) {
        console.log(err);
        res.send({ Success: false, MessageCode: 'PA000', Message: err.message });
    }
});
app.get('/', async (req, res) => {

    let query = 'SELECT * FROM users';
    var con = await mysqlConnect();
    con.query(query, (err, result, fields) => {
        console.log('query successfull');
        con.destroy();
        res.send(result);
    })
});

//=============================Functions=================================
function insertMessageToDB(users_id, title, sender, body, time, image_url, sent, connection) {
    return new Promise(async resolve => {
        var sql = 'INSERT into `message` (`users_id`, `title`, `sender`, `body`, `time`, `image_url`, `sent`) VALUES (?)';
        var values = [[users_id, title, sender, body, time, image_url, sent]];
        connection.query(sql, values, async (err, result) => {
            if (err) {
                connection.destroy();
                resolve(err);
            }
            else {
                connection.destroy();
                //console.log(result.insertId);
                resolve(result);
            }
        });
    })
}
function sendMessage(tokens, title, body, sender, image, time, type) {
    return new Promise(async resolve => {
        var _type = type;
        if (!_type || _type == '') {
            _type = 'Defualt';
        }

        var message = {
            registration_ids: tokens,
            notification: {
                title: title,
                body: body
            },
            data: {  //you can send only notification or only data(or include both)
                sender: sender,
                image: image,
                time: time,
                type: _type
            }
        };

        fcm.send(message, function (err, response) {
            if (err) {
                resolve(err);
            } else {
                resolve(response);
            }
        });
    }
    )
}
function sendEmailCode(email, code) {
    return new Promise(resolve => {

        const transporter = nodemailer.createTransport({
            service: 'gmail',
            host: 'smtp.gmail.com',
            port: 465,
            secure: true,  //true for 465 port, false for other ports
            auth: {
                user: 'email',
                pass: 'password'
            }
        });
        const mailOptions = {
            from: '"Push That" <email>', // sender address
            to: email, // list of receivers
            subject: 'Push over Verification Code', // Subject line
            text: 'code: ' + code, // plain text body
            html: '<b>code: ' + code + '</b>' // html body
        };
        transporter.sendMail(mailOptions, (error, info) => {
            if (error) {
                console.log(error);
            } else {
                resolve(true);
            }
        });

    });
}
function QueryExecution(query, value, connection) {
    return new Promise(async resolve => {
        await connection.query(query, value, async function (err, result, fields) {
            if (err) {
                // console.log(value);
                resolve(err);
            }
            else {
                console.log('query executed');
                // console.log(result);
                resolve(result);
            }
        });

    });
}
function newToken(pushKey) {

    var token = jwt.sign({ pushThatkey: pushKey }, secret, { expiresIn: '24hr' }); //expire in 24hr
    return token;
}
function varifyToken(_token, pushKey) {
    return new Promise(
        resolve => {
            var token = _token;
            if (token.startsWith('Bearer ')) {
                // Remove Bearer from string
                token = token.slice(7, token.length);
            }

            if (token) {
                jwt.verify(token, secret, (err, decoded) => {
                    if (err) {

                        var obj = { Status: false, MessageCode: 'PA117', Message: 'Token is expired' };
                        resolve(obj);
                    }
                    else {
                        if (pushKey != decoded.pushThatkey) {
                            var obj = { Status: false, MessageCode: 'PA118', Message: 'Token is invalid / unauthorized' };
                            resolve(obj);
                        }
                    }
                    var obj = { Status: true, MessageCode: 'PA119', Message: 'Token is varified' };
                    resolve(obj);
                });
            }
        }
    )
}

//============================Cron=======================================
var licenseJob = new CronJob('* */59 */23 * * *', async function () {
    console.log('Cron in called at 11:59 am');
    try {
        var connection = await mysqlConnect();
        var targetRow = await QueryExecution('SELECT * from `licensing` WHERE licensing = ? AND expired = ?',
            ['TRIAL', 1], connection);
        if (targetRow.length == 0) {
            connection.destroy();
            return;
        }
        for (var i of targetRow) {
            var endDate = i.expired_at.toString();
            var startDate = i.created_at.toString();
            var days = Math.floor((Date.parse(endDate.replace(/-/g, "/")) - Date.parse(startDate.replace(/-/g, "/"))) / 86400000);
            if (days >= 7) {
                var expiredRow = await QueryExecution('UPDATE `licensing` SET licensing = "EXPIRED", expired = 0 WHERE users_id = ?',
                    [i.users_id], connection);
                //console.log(expiredRow);
            }
        }
        connection.destroy();
    } catch (err) {
        console.log(err);
        res.send({ Success: false, MessageCode: 'PA000', Message: err.message });
    }

});
licenseJob.start();

var codeJob = new CronJob('* */59 */23 * * *', async function () {
    console.log('Cron in called at 10:59 am');
    try {
        var time = new Date();
        var time1 = time.toISOString();
        var timestamp = time1.substr(0, time1.length - 1);

        // console.log(timestamp.toString());

        var connection = await mysqlConnect();

        connection.query('SELECT * FROM `forget_password`', async function (err, result, fields) {
            if (err) return err;
            else {
                //console.log(result);
                if (result.length == 0) {
                    connection.destroy();
                    return;
                }
                for (var i of result) {
                    const diffTime = Math.abs(time - i.time);
                    const diffDays = Math.ceil(diffTime / 86400000);
                    // console.log(diffDays);
                    if (diffDays >= 1) {
                        var expiredRow = await QueryExecution('DELETE FROM `forget_password` WHERE id = ?',
                            [i.id], connection);
                        // console.log(expiredRow);
                    }
                }
                connection.destroy();
            }
        });
    } catch (err) {
        console.log(err);
        res.send({ Success: false, MessageCode: 'PA000', Message: err.message });
    }
});
codeJob.start();

app.listen(port, () => { console.log(`Application is listening on port ${port}.`) });
