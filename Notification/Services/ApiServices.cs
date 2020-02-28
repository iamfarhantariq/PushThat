using Newtonsoft.Json;
using Plugin.Connectivity;
using Notification.Helpers;
using Notification.Model;
using Notification.Strings;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.IO;

namespace Notification.Services
{
    public class ApiServices
    {
        private static UserClass data;

        public static async Task<object> Post(string api, object Model)
        {
            HttpClient httpClient = null;
            try
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    var hostAccess = PingHost();
                    if (hostAccess)
                    {
                        httpClient = new HttpClient();
                        var json = JsonConvert.SerializeObject(Model);
                        HttpContent httpContent = new StringContent(json);
                        if (Application.Current.Properties.ContainsKey(NotificationStrings.Token))
                        {
                            var token = (string)Application.Current.Properties[NotificationStrings.Token];
                            httpClient.DefaultRequestHeaders.Add("Authorization", token);
                        }
                        httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        var response = await httpClient.PostAsync(WebConfig.BaseUrl + api, httpContent);
                        //await Task.Delay(300);
                        if (response.StatusCode == HttpStatusCode.BadRequest)
                        {
                            RefreshToken();
                        }
                        if (response.IsSuccessStatusCode)
                        {
                            var serializeresponse = await response.Content.ReadAsStringAsync();
                            return serializeresponse;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        DependencyService.Get<IMessage>().ShortAlert("Server is unreachable");
                        return null;
                    }
                }
                else
                {
                    DependencyService.Get<IMessage>().ShortAlert("No Internet Connection");
                    return null;
                }
            }
            catch (WebException)
            {
                //DependencyService.Get<IMessage>().LongAlert(ex.ToString());
                return null;
            }
            catch (IOException)
            {
                //DependencyService.Get<IMessage>().LongAlert(ex.ToString());
                return null;
            }
            catch (Exception)
            {
                //DependencyService.Get<IMessage>().LongAlert(ex.ToString());
                return null;
            }
            finally
            {
                if (httpClient != null)
                    httpClient.Dispose();
            }
        }

        private async static void RefreshToken()
        {
            UserClass _obj = new UserClass()
            {
                Email = (string)Application.Current.Properties[NotificationStrings.Email],
                Password = (string)Application.Current.Properties[NotificationStrings.Password]
            };
            var status = await Post(WebConfig.RefreshToken, _obj);
            if (status != null)
                data = JsonConvert.DeserializeObject<UserClass>(status.ToString());
            if (data != null && data.Success == true)
            {
                Application.Current.Properties[NotificationStrings.Token] = data.Token;
                await Application.Current.SavePropertiesAsync();
            }
        }
        public static bool PingHost()
        {
            bool pingable = false;
            Ping pinger = null;

            try
            {
                pinger = new Ping();
                PingReply reply = pinger.Send("35.236.14.215");
                pingable = reply.Status == IPStatus.Success;
                //Task.Delay(250);
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }

            return pingable;
        }

    }
}
