using System.Net.Http.Headers;
using LineBotMessage.Dtos;
using LineBotMessage.Enum;
using LineBotMessage.Providers;
using System.Text;
using LineBotMessage.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Newtonsoft.Json;
using System.Net.WebSockets;

namespace LineBotMessage.Domain
{
    public class LineBotService
    {

        // 貼上 messaging api channel 中的 accessToken & secret
        private readonly string channelAccessToken = "/PwKqaK8nQ02MVsw2FnSZ6q0bCjrwaMUZz4LUhqbXvXeAzC3urypi8c1/2BfTxwpXzKUaVtyy44t4gUeUqMYGbO2XY+MXG1brtgCLqYFlZt0SiWgl0j7xx1xLOEHl+MPz4ehhIjIb7/p2nZkD6w3oAdB04t89/1O/w1cDnyilFU=";
        private readonly string channelSecret = "d6cb0eeb502d5bac960b89a55f249cd4";

        private readonly string replyMessageUri = "https://api.line.me/v2/bot/message/reply";
        private readonly string broadcastMessageUri = "https://api.line.me/v2/bot/message/broadcast";


        private static HttpClient client = new HttpClient();
        private readonly JsonProvider _jsonProvider = new JsonProvider();
        private readonly LinebotAPIContext _context = new LinebotAPIContext();

        public LineBotService() { }

        public LineBotService(LinebotAPIContext context)
        {
            _context = context;
        }



        /// <summary>
        /// 接收 webhook event 處理
        /// </summary>
        /// <param name="requestBody"></param>

        public void ReceiveWebhook(WebhookRequestBodyDto requestBody)
        {
            dynamic replyMessage;
            foreach (var eventObject in requestBody.Events)
            {
                switch (eventObject.Type)
                {
                    case WebhookEventTypeEnum.Message:
                        if (eventObject.Message.Type == MessageTypeEnum.Text)
                            ReceiveMessageWebhookEvent(eventObject);
                        break;

                    case WebhookEventTypeEnum.Postback:
                        ReceivePostbackWebhookEvent(eventObject);
                        break;

                    case WebhookEventTypeEnum.Follow:
                        replyMessage = new ReplyMessageRequestDto<TemplateMessageDto<ButtonsTemplateDto>>
                        {
                            ReplyToken = eventObject.ReplyToken,
                            Messages = new List<TemplateMessageDto<ButtonsTemplateDto>>
                            {
                                new TemplateMessageDto<ButtonsTemplateDto>
                                {
                                    AltText = "資訊服務組歡迎您",
                                    Template = new ButtonsTemplateDto
                                    {
                                        ThumbnailImageUrl = "https://i.imgur.com/lAhzp6L.png?2",
                                        ImageAspectRatio = TemplateImageAspectRatioEnum.Rectangle,
                                        ImageSize = TemplateImageSizeEnum.Contain,
                                        Title = "親愛的用戶您好，歡迎您使用分機查尋系統",
                                        Text = "請選擇需要查尋的資訊",
                                        Actions = new List<ActionDto>
                                        {
                                            new ActionDto
                                            {
                                                Type = ActionTypeEnum.Postback,
                                                Data = "dataType=staffs",
                                                Label = "職員",
                                                DisplayText = "職員"
                                            },
                                            new ActionDto
                                            {
                                                Type = ActionTypeEnum.Postback,
                                                Data = "dataType=departments",
                                                Label = "單位",
                                                DisplayText = "單位"
                                            },
                                            new ActionDto
                                            {
                                                Type = ActionTypeEnum.Postback,
                                                Data = "dataType=extentionNumbers",
                                                Label = "分機",
                                                DisplayText = "分機"
                                            }
                                        }
                                    }
                                }
                            }
                        };

                        ReplyMessageHandler(replyMessage);
                        break;

                }
            }
        }
        public void ReceivePostbackWebhookEvent(WebhookEventDto eventDto)
        {
            dynamic replyMessage = new ReplyMessageRequestDto<BaseMessageDto>();

            switch (eventDto.Postback.Data)
            {
                case "dataType=staffs":
                    // 回傳使用職員名稱查詢的功能
                    replyMessage = CreateTextMessage("請輸入職員名稱，例:陳小明", eventDto);
                    break;

                case "dataType=departments":
                    // 回傳使用單位查詢的功能
                    replyMessage = CreateTextMessage("請輸入單位名稱，例：資訊服務組", eventDto);
                    break;

                case "dataType=extentionNumbers":
                    // 回傳使用分機查詢的功能
                    replyMessage = CreateTextMessage("請輸入分機4碼", eventDto);
                    break;

                case "dataType=job":
                    // 回傳業務職掌
                    replyMessage = CreateFlexMessageFromFile("C:\\Users\\caesa\\source\\repos\\Linebot_WebAPI\\LineBotMessage\\LineBotMessage\\JsonMessages\\richmenuJob.json", eventDto, "業務職掌");
                    break;

                case "dataType=service":
                    // 回傳業務職掌
                    replyMessage = CreateFlexMessageFromFile("C:\\Users\\caesa\\source\\repos\\Linebot_WebAPI\\LineBotMessage\\LineBotMessage\\JsonMessages\\richmenuService.json", eventDto, "服務項目");
                    break;

                case "ex_1":
                    //回傳分機
                    replyMessage = CreateTextMessage("分機號碼: 3310 3321", eventDto);
                    break;

                case "ex_2":
                    //回傳分機
                    replyMessage = CreateTextMessage("分機號碼: 3311", eventDto);
                    break;

                case "ex_3":
                    //回傳分機
                    replyMessage = CreateTextMessage("分機號碼: 3313", eventDto);
                    break;

                case "ex_4":
                    //回傳分機
                    replyMessage = CreateTextMessage("分機號碼: 3312", eventDto);
                    break;

                case "email_1":
                    //回傳分機
                    replyMessage = CreateTextMessage("Email: norrith@ntus.edu.tw", eventDto);
                    break;

                case "email_2":
                    //回傳分機
                    replyMessage = CreateTextMessage("Email: rex@ntus.edu.tw", eventDto);
                    break;

                case "email_3":
                    //回傳分機
                    replyMessage = CreateTextMessage("Email: zonghao.xie@ntus.edu.tw", eventDto);
                    break;

                case "email_4":
                    //回傳分機
                    replyMessage = CreateTextMessage("Email: ", eventDto);
                    break;

                default:
                    break;
            }

            ReplyMessageHandler(replyMessage);
        }

        private ReplyMessageRequestDto<TextMessageDto> CreateTextMessage(string text, WebhookEventDto eventDto)
        {
            return new ReplyMessageRequestDto<TextMessageDto>
            {
                ReplyToken = eventDto.ReplyToken,
                Messages = new List<TextMessageDto>
                {
                    new TextMessageDto() { Text = text }
                }
            };
        }

        private ReplyMessageRequestDto<FlexMessageDto<FlexCarouselContainerDto>> CreateFlexMessageFromFile(string filePath, WebhookEventDto eventDto, string altText)
        {
            var json = File.ReadAllText(filePath);
            return new ReplyMessageRequestDto<FlexMessageDto<FlexCarouselContainerDto>>
            {
                ReplyToken = eventDto.ReplyToken,
                Messages = new List<FlexMessageDto<FlexCarouselContainerDto>>
                {
                    new FlexMessageDto<FlexCarouselContainerDto>()
                    {
                        AltText = altText,
                        Contents = _jsonProvider.Deserialize<FlexCarouselContainerDto>(json)
                    }
                }
            };
        }               

        public void ReceiveMessageWebhookEvent(WebhookEventDto eventDto)
        {
            var replyMessage = new ReplyMessageRequestDto<TextMessageDto>();
            string messageText = "";

            switch (eventDto.Message.Type)
            {
                case MessageTypeEnum.Text:

                    // 將使用者輸入的字串存到 userInput 變數
                    string userInput = eventDto.Message.Text.Trim();

                    // 使用 LINQ 查詢資料庫中是否有對應的 ExtentionNumber、Staff 或 Department 記錄
                    var extentionnumber = _context.Staffs
                        .Include(e => e.StaffsDepartmentNavigation)
                        .FirstOrDefault(pext => pext.StaffsExtentionnumber.Equals(userInput));

                    var multiExtentionnumber = extentionnumber == null ?
                        _context.StaffsMultiExtentionnumbers
                        .Include(e => e.StaffsDepartmentNavigation)
                        .FirstOrDefault(pext =>
                        pext.StaffsExtentionnumber1.Equals(userInput) ||
                        pext.StaffsExtentionnumber2.Equals(userInput) ||
                        pext.StaffsExtentionnumber3.Equals(userInput)) :
                        null;

                    var staff = _context.Staffs
                        .Include(s => s.StaffsDepartmentNavigation)
                        .FirstOrDefault(pext => pext.StaffsName.Equals(userInput));

                    var staffMulti = staff == null ?
                        _context.StaffsMultiExtentionnumbers
                        .Include(s => s.StaffsDepartmentNavigation)
                        .FirstOrDefault(pext => pext.StaffsName.Equals(userInput)) :
                        null;
                                     
                    var dept = from department in _context.Departments
                               join staff_dept in _context.Staffs on department.Id equals staff_dept.StaffsDepartment into staffsGroup
                               from staff_dept in staffsGroup.DefaultIfEmpty()
                               join staffMulti_dept in _context.StaffsMultiExtentionnumbers on department.Id equals staffMulti_dept.StaffsDepartment into staffsMultiGroup
                               from staffMulti_dept in staffsMultiGroup.DefaultIfEmpty()
                               where department.DepartmentsName == userInput
                               select new { staff_dept, staffMulti_dept, department.DepartmentsName };


                    // 判斷是否找到對應的 PhoneExtentionNumber、Staff 或 Department 記錄
                    if (staff != null)
                    {
                        messageText = $"所屬單位: {staff.StaffsDepartmentNavigation.DepartmentsName}\n分機號碼: {staff.StaffsExtentionnumber}";
                    }
                    else if (staffMulti != null)                      
                    {
                        string extNumbers = "";
                        if (staffMulti.StaffsExtentionnumber1 != null)
                        {
                            extNumbers += staffMulti.StaffsExtentionnumber1 + " ";
                        }
                        if (staffMulti.StaffsExtentionnumber2 != null)
                        {
                            extNumbers += staffMulti.StaffsExtentionnumber2 + " ";
                        }
                        if (staffMulti.StaffsExtentionnumber3 != null)
                        {
                            extNumbers += staffMulti.StaffsExtentionnumber3 + " ";
                        }
                        messageText = $"所屬單位: {staffMulti.StaffsDepartmentNavigation.DepartmentsName}\n分機號碼: {extNumbers}";
                    }
                    else if (extentionnumber != null)
                    {
                        messageText = $"職員名稱: {extentionnumber.StaffsName} \n所屬單位: {extentionnumber.StaffsDepartmentNavigation.DepartmentsName}";
                    }
                    else if(multiExtentionnumber != null)
                    {
                        messageText = $"職員名稱: {multiExtentionnumber.StaffsName} \n所屬單位: {multiExtentionnumber.StaffsDepartmentNavigation.DepartmentsName}";
                    }
                    else if (dept.Any())
                    {
                        messageText = "";
                        foreach (var staffcount in dept)
                        {
                            messageText += $"職員名稱: {staffcount.staff_dept.StaffsName}\n分機: {staffcount.staff_dept.StaffsExtentionnumber}\n";
                            if (staffcount.staffMulti_dept != null)
                            {
                                string extNumbers = "";
                                if (staffcount.staffMulti_dept.StaffsExtentionnumber1 != null)
                                {
                                    extNumbers += staffcount.staffMulti_dept.StaffsExtentionnumber1 + " ";
                                }
                                if (staffcount.staffMulti_dept.StaffsExtentionnumber2 != null)
                                {
                                    extNumbers += staffcount.staffMulti_dept.StaffsExtentionnumber2 + " ";
                                }
                                if (staffcount.staffMulti_dept.StaffsExtentionnumber3 != null)
                                {
                                    extNumbers += staffcount.staffMulti_dept.StaffsExtentionnumber3 + " ";
                                }
                                messageText += $"職員名稱: {staffcount.staffMulti_dept.StaffsName}\n分機: {extNumbers}";
                            }
                            messageText += "\n\n";
                        }
                    }
                    else
                    {
                        replyMessage = new ReplyMessageRequestDto<TextMessageDto>
                        {
                            ReplyToken = eventDto.ReplyToken,
                            Messages = new List<TextMessageDto>
                    {
                        new TextMessageDto(){Text = $"找不到分機號碼，職員名字或單位名稱為 {userInput}"}
                    }
                        };
                    }

                    if (!string.IsNullOrEmpty(messageText))
                    {
                        var messageDto = new TextMessageDto { Text = messageText };
                        // 將訊息物件加入回應訊息列表
                        replyMessage = new ReplyMessageRequestDto<TextMessageDto>
                        {
                            ReplyToken = eventDto.ReplyToken,
                            Messages = new List<TextMessageDto> { messageDto }
                        };
                    }

                    break;
            }

            ReplyMessageHandler(replyMessage);
        }

       
       
        /// <summary>
        /// 接收到廣播請求時，在將請求傳至 Line 前多一層處理，依據收到的 messageType 將 messages 轉換成正確的型別，這樣 Json 轉換時才能正確轉換。
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="requestBody"></param>
        public void BroadcastMessageHandler(string messageType, object requestBody)
        {
            string strBody = requestBody.ToString();
            dynamic messageRequest = new BroadcastMessageRequestDto<BaseMessageDto>();
            switch (messageType)
            {
                case MessageTypeEnum.Text:
                    messageRequest = _jsonProvider.Deserialize<BroadcastMessageRequestDto<TextMessageDto>>(strBody);
                    break;

                case MessageTypeEnum.Sticker:
                    messageRequest = _jsonProvider.Deserialize<BroadcastMessageRequestDto<StickerMessageDto>>(strBody);
                    break;

                case MessageTypeEnum.Image:
                    messageRequest = _jsonProvider.Deserialize<BroadcastMessageRequestDto<ImageMessageDto>>(strBody);
                    break;

                case MessageTypeEnum.Video:
                    messageRequest = _jsonProvider.Deserialize<BroadcastMessageRequestDto<VideoMessageDto>>(strBody);
                    break;

                case MessageTypeEnum.Audio:
                    messageRequest = _jsonProvider.Deserialize<BroadcastMessageRequestDto<AudioMessageDto>>(strBody);
                    break;

                case MessageTypeEnum.Location:
                    messageRequest = _jsonProvider.Deserialize<BroadcastMessageRequestDto<LocationMessageDto>>(strBody);
                    break;

                case MessageTypeEnum.Imagemap:
                    messageRequest = _jsonProvider.Deserialize<BroadcastMessageRequestDto<ImagemapMessageDto>>(strBody);
                    break;

                case MessageTypeEnum.FlexBubble:
                    messageRequest = _jsonProvider.Deserialize<BroadcastMessageRequestDto<FlexMessageDto<FlexBubbleContainerDto>>>(strBody);
                    break;

                case MessageTypeEnum.FlexCarousel:
                    messageRequest = _jsonProvider.Deserialize<BroadcastMessageRequestDto<FlexMessageDto<FlexCarouselContainerDto>>>(strBody);
                    break;
            }
            BroadcastMessage(messageRequest);

        }

        /// <summary>
        /// 將廣播訊息請求送到 Line
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        public async void BroadcastMessage<T>(BroadcastMessageRequestDto<T> request)
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", channelAccessToken); //帶入 channel access token
            var json = _jsonProvider.Serialize(request);
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(broadcastMessageUri),
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var response = await client.SendAsync(requestMessage);
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// 接收到回覆請求時，在將請求傳至 Line 前多一層處理(目前為預留)
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="requestBody"></param>
        public void ReplyMessageHandler<T>(ReplyMessageRequestDto<T> requestBody)
        {
            ReplyMessage(requestBody);
        }

        /// <summary>
        /// 將回覆訊息請求送到 Line
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        public async void ReplyMessage<T>(ReplyMessageRequestDto<T> request)
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", channelAccessToken); //帶入 channel access token
            var json = _jsonProvider.Serialize(request);
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(replyMessageUri),
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var response = await client.SendAsync(requestMessage);
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }

    }
}

