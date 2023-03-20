using System.Net.Http.Headers;
using LineBotMessage.Dtos;
using LineBotMessage.Enum;
using LineBotMessage.Providers;
using System.Text;
using LineBotMessage.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Infrastructure;

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
                                    AltText = "這是按鈕模板訊息",
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
                    replyMessage = new ReplyMessageRequestDto<TextMessageDto>
                    {
                        ReplyToken = eventDto.ReplyToken,
                        Messages = new List<TextMessageDto>
                        {
                            new TextMessageDto(){Text = $"請輸入職員名稱，例:陳小明、小明、陳組長"}
                        }
                    };

                    break;

                case "dataType=departments":
                    // 回傳使用單位查詢的功能
                    replyMessage = new ReplyMessageRequestDto<TextMessageDto>
                    {
                        ReplyToken = eventDto.ReplyToken,
                        Messages = new List<TextMessageDto>
                        {
                         new TextMessageDto(){Text = $"請輸入單位名稱，例：資訊服務組、資服組、資訊"}
                        }
                    };

                    break;

                case "dataType=extentionNumbers":
                    // 回傳使用分機查詢的功能
                    replyMessage = new ReplyMessageRequestDto<TextMessageDto>
                    {
                        ReplyToken = eventDto.ReplyToken,
                        Messages = new List<TextMessageDto>
                        {
                         new TextMessageDto(){Text = $"請輸入分機4碼"}
                        }
                    };

                    break;
            }

            ReplyMessageHandler(replyMessage);
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
        //public void ReceiveMessageWebhookEvent(WebhookEventDto eventDto)
        //{
        //    dynamic replyMessage = new ReplyMessageRequestDto<BaseMessageDto>();

        //    switch (eventDto.Message.Type)
        //    {
        //        case MessageTypeEnum.Text:
        //            if (eventDto.Message.Text == "職員")
        //            {
        //                replyMessage = new ReplyMessageRequestDto<TextMessageDto>
        //                {
        //                    ReplyToken = eventDto.ReplyToken,
        //                    Messages = new List<TextMessageDto>
        //                    {
        //                        new TextMessageDto(){Text = $"請輸入單位名稱，例:資訊、資服組、資訊服務組"}
        //                    }
        //                };
        //            }
        //            //關鍵字:單位
        //            if (eventDto.Message.Text == "單位")
        //            {
        //                replyMessage = new ReplyMessageRequestDto<TextMessageDto>
        //                {
        //                    ReplyToken = eventDto.ReplyToken,
        //                    Messages = new List<TextMessageDto>
        //                    {
        //                        new TextMessageDto(){Text = $"請輸入單位名稱，例:資訊、資服組、資訊服務組"}
        //                    }
        //                };
        //            }
        //            //關鍵字:分機
        //            if (eventDto.Message.Text == "分機")
        //            {
        //                replyMessage = new ReplyMessageRequestDto<TextMessageDto>
        //                {
        //                    ReplyToken = eventDto.ReplyToken,
        //                    Messages = new List<TextMessageDto>
        //                    {
        //                        new TextMessageDto(){Text = $"請輸入分機4碼"}
        //                    }
        //                };
        //            };

        //            break;
        //    }
        //    ReplyMessageHandler(replyMessage);
        //}

        //public void ReceiveWebhookEvent(WebhookEventDto eventDto) 
        //{
        //    dynamic replyMessage = new ReplyMessageRequestDto<BaseMessageDto>();

        //    switch(eventDto.Type) 
        //    {
        //        ExtentionNumber? ExtentionNumber = await _context.ExtentionNumbers.FirstOrDefaultAsync(pext => pext.Name == sName);
        //    }
        //    ReplyMessageHandler(replyMessage);
        //    break;
        //}

        //public void ReceiveWebhook(WebhookRequestBodyDto requestBody)
        //{
        //    dynamic replyMessage;
        //    foreach (var eventObject in requestBody.Events)
        //    {
        //        switch (eventObject.Type)
        //        {
        //            case WebhookEventTypeEnum.Message:
        //                if (eventObject.Message.Type == MessageTypeEnum.Text)
        //                    ReceiveMessageWebhookEvent(eventObject);
        //                break;
        //            case WebhookEventTypeEnum.Unsend:
        //                Console.WriteLine($"使用者{eventObject.Source.UserId}在聊天室收回訊息！");
        //                break;
        //            case WebhookEventTypeEnum.Follow:
        //                Console.WriteLine($"使用者{eventObject.Source.UserId}將我們新增為好友！");
        //                break;
        //            case WebhookEventTypeEnum.Unfollow:
        //                Console.WriteLine($"使用者{eventObject.Source.UserId}封鎖了我們！");
        //                break;
        //            case WebhookEventTypeEnum.Join:
        //                Console.WriteLine("我們被邀請進入聊天室了！");
        //                break;
        //            case WebhookEventTypeEnum.Leave:
        //                Console.WriteLine("我們被聊天室踢出了");
        //                break;
        //            case WebhookEventTypeEnum.MemberJoined:
        //                string joinedMemberIds = "";
        //                foreach (var member in eventObject.Joined.Members)
        //                {
        //                    joinedMemberIds += $"{member.UserId} ";
        //                }
        //                Console.WriteLine($"使用者{joinedMemberIds}加入了群組！");
        //                break;
        //            case WebhookEventTypeEnum.MemberLeft:
        //                string leftMemberIds = "";
        //                foreach (var member in eventObject.Left.Members)
        //                {
        //                    leftMemberIds += $"{member.UserId} ";
        //                }
        //                Console.WriteLine($"使用者{leftMemberIds}離開了群組！");
        //                break;
        //            case WebhookEventTypeEnum.Postback:
        //                Console.WriteLine($"使用者{eventObject.Source.UserId}觸發了postback事件");
        //                break;
        //            case WebhookEventTypeEnum.VideoPlayComplete:
        //                replyMessage = new ReplyMessageRequestDto<TextMessageDto>()
        //                {
        //                    ReplyToken = eventObject.ReplyToken,
        //                    Messages = new List<TextMessageDto>
        //                    {
        //                        new TextMessageDto(){Text = $"使用者您好，謝謝您收看我們的宣傳影片，祝您身體健康萬事如意 !"}
        //                    }
        //                };
        //                ReplyMessageHandler(replyMessage);
        //                break;
        //        }
        //    }
        //}

        //private void ReceiveMessageWebhookEvent(WebhookEventDto eventDto)
        //{
        //    dynamic replyMessage = new ReplyMessageRequestDto<BaseMessageDto>();

        //    switch (eventDto.Message.Type)
        //    {
        //        // 收到文字訊息
        //        case MessageTypeEnum.Text:
        //            // 關鍵字 : "測試"
        //            if (eventDto.Message.Text == "測試")
        //            {
        //                // 回覆文字訊息並挾帶 quick reply button
        //                replyMessage = new ReplyMessageRequestDto<TextMessageDto>
        //                {
        //                    ReplyToken = eventDto.ReplyToken,
        //                    Messages = new List<TextMessageDto>
        //                    {
        //                        new TextMessageDto
        //                        {
        //                            Text = "QuickReply 測試訊息",
        //                            QuickReply = new QuickReplyItemDto
        //                            {
        //                                Items = new List<QuickReplyButtonDto>
        //                                {
        //                                    // message action
        //                                    new QuickReplyButtonDto {
        //                                        Action = new ActionDto {
        //                                            Type = ActionTypeEnum.Message,
        //                                            Label = "message 測試" ,
        //                                            Text = "測試"
        //                                        }
        //                                    },
        //                                    // uri action
        //                                    new QuickReplyButtonDto {
        //                                        Action = new ActionDto {
        //                                            Type = ActionTypeEnum.Uri,
        //                                            Label = "uri 測試" ,
        //                                            Uri = "https://www.appx.com.tw"
        //                                        }
        //                                    },
        //                                     // 使用 uri schema 分享 Line Bot 資訊
        //                                    new QuickReplyButtonDto {
        //                                        Action = new ActionDto {
        //                                            Type = ActionTypeEnum.Uri,
        //                                            Label = "分享 Line Bot 資訊" ,
        //                                            Uri = "https://line.me/R/nv/recommendOA/@089yvykp"
        //                                        }
        //                                    },
        //                                    // postback action
        //                                    new QuickReplyButtonDto {
        //                                        Action = new ActionDto {
        //                                            Type = ActionTypeEnum.Postback,
        //                                            Label = "postback 測試" ,
        //                                            Data = "quick reply postback action" ,
        //                                            DisplayText = "使用者傳送 displayTex，但不會有 Webhook event 產生。",
        //                                            InputOption = PostbackInputOptionEnum.OpenKeyboard,
        //                                            FillInText = "第一行\n第二行"
        //                                        }
        //                                    },
        //                                    // datetime picker action
        //                                    new QuickReplyButtonDto {
        //                                        Action = new ActionDto {
        //                                        Type = ActionTypeEnum.DatetimePicker,
        //                                        Label = "日期時間選擇",
        //                                            Data = "quick reply datetime picker action",
        //                                            Mode = DatetimePickerModeEnum.Datetime,
        //                                            Initial = "2022-09-30T19:00",
        //                                            Max = "2022-12-31T23:59",
        //                                            Min = "2021-01-01T00:00"
        //                                        }
        //                                    },
        //                                    // camera action
        //                                    new QuickReplyButtonDto {
        //                                        Action = new ActionDto {
        //                                            Type = ActionTypeEnum.Camera,
        //                                            Label = "開啟相機"
        //                                        }
        //                                    },
        //                                    // camera roll action
        //                                    new QuickReplyButtonDto {
        //                                        Action = new ActionDto {
        //                                            Type = ActionTypeEnum.CameraRoll,
        //                                            Label = "開啟相簿"
        //                                        }
        //                                    },
        //                                    // location action
        //                                    new QuickReplyButtonDto {
        //                                        Action = new ActionDto {
        //                                            Type = ActionTypeEnum.Location,
        //                                            Label = "開啟位置"
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                };
        //            }
        //            // 關鍵字 : "Sender"
        //            if (eventDto.Message.Text == "Sender")
        //            {
        //                replyMessage = new ReplyMessageRequestDto<TextMessageDto>
        //                {
        //                    ReplyToken = eventDto.ReplyToken,
        //                    Messages = new List<TextMessageDto>
        //                    {
        //                        new TextMessageDto
        //                        {
        //                            Text = "你好，我是客服人員 1號",
        //                            Sender = new SenderDto
        //                            {
        //                                Name = "客服人員 1號",
        //                                IconUrl = "https://3a8e-114-37-157-213.jp.ngrok.io/UploadFiles/man.png"
        //                            }
        //                        },
        //                        new TextMessageDto
        //                        {
        //                            Text = "你好，我是客服人員 2號",
        //                            Sender = new SenderDto
        //                            {
        //                                Name = "客服人員 2號",
        //                                IconUrl = "https://3a8e-114-37-157-213.jp.ngrok.io/UploadFiles/gamer.png"
        //                            }
        //                        }
        //                    }
        //                };
        //            }
        //            // 關鍵字 : "Buttons"
        //            if (eventDto.Message.Text == "Buttons")
        //            {
        //                replyMessage = new ReplyMessageRequestDto<TemplateMessageDto<ButtonsTemplateDto>>
        //{
        //    ReplyToken = eventDto.ReplyToken,
        //    Messages = new List<TemplateMessageDto<ButtonsTemplateDto>>
        //    {
        //        new TemplateMessageDto<ButtonsTemplateDto>
        //        {
        //            AltText = "這是按鈕模板訊息",
        //            Template = new ButtonsTemplateDto
        //            {
        //                ThumbnailImageUrl = "https://i.imgur.com/CP6ywwc.jpg",
        //                ImageAspectRatio = TemplateImageAspectRatioEnum.Rectangle,
        //                ImageSize = TemplateImageSizeEnum.Cover,
        //                Title = "親愛的用戶您好，歡迎您使用本美食推薦系統",
        //                Text = "請選擇今天想吃的主食，系統會推薦附近的餐廳給您。",
        //                Actions = new List<ActionDto>
        //                {
        //                    new ActionDto
        //                    {
        //                        Type = ActionTypeEnum.Postback,
        //                        Data = "foodType=sushi",
        //                        Label = "壽司",
        //                        DisplayText = "壽司"
        //                    },
        //                    new ActionDto
        //                    {
        //                        Type = ActionTypeEnum.Postback,
        //                        Data = "foodType=hot-pot",
        //                        Label = "火鍋",
        //                        DisplayText = "火鍋"
        //                    },
        //                    new ActionDto
        //                    {
        //                        Type = ActionTypeEnum.Postback,
        //                        Data = "foodType=steak",
        //                        Label = "牛排",
        //                        DisplayText = "牛排"
        //                    },
        //                    new ActionDto
        //                    {
        //                        Type = ActionTypeEnum.Postback,
        //                        Data = "foodType=next",
        //                        Label = "下一個",
        //                        DisplayText = "下一個"
        //                    }
        //                }
        //            }
        //        }
        //    }
        //};
        //            }
        //            // 關鍵字 : "Confirm"
        //            if (eventDto.Message.Text == "Confirm")
        //            {
        //                replyMessage = new ReplyMessageRequestDto<TemplateMessageDto<ConfirmTemplateDto>>
        //                {
        //                    ReplyToken = eventDto.ReplyToken,
        //                    Messages = new List<TemplateMessageDto<ConfirmTemplateDto>>
        //                    {
        //                        new TemplateMessageDto<ConfirmTemplateDto>
        //                        {
        //                            AltText = "這是確認模組訊息",
        //                            Template = new ConfirmTemplateDto
        //                            {
        //                                Text = "請問您是否喜歡本產品?\n(產品編號123)",
        //                                Actions = new List<ActionDto>
        //                                {
        //                                    new ActionDto
        //                                    {
        //                                        Type = ActionTypeEnum.Postback,
        //                                        Data = "id=123&like=yes",
        //                                        Label = "喜歡",
        //                                        DisplayText = "喜歡",
        //                                    },
        //                                    new ActionDto
        //                                    {
        //                                        Type = ActionTypeEnum.Postback,
        //                                        Data = "id=123&like=no",
        //                                        Label = "不喜歡",
        //                                        DisplayText = "不喜歡",
        //                                    }
        //                                }

        //                            }
        //                        }

        //                    }
        //                };
        //            }
        //            // 關鍵字 : "Carousel"
        //            if(eventDto.Message.Text == "Carousel")
        //            {
        //                replyMessage = new ReplyMessageRequestDto<TemplateMessageDto<CarouselTemplateDto>>
        //                {
        //                    ReplyToken = eventDto.ReplyToken,
        //                    Messages = new List<TemplateMessageDto<CarouselTemplateDto>>
        //                    {
        //                        new TemplateMessageDto<CarouselTemplateDto>
        //                        {
        //                            AltText = "這是輪播訊息",
        //                            Template = new CarouselTemplateDto
        //                            {
        //                                Columns = new List<CarouselColumnObjectDto>
        //                                {
        //                                    //column objects
        //                                    new CarouselColumnObjectDto
        //                                    {
        //                                        ThumbnailImageUrl = "https://www.apple.com/v/iphone-14-pro/a/images/meta/iphone-14-pro_overview__e2a7u9jy63ma_og.png",
        //                                        Title = "全新上市 iPhone 14 Pro",
        //                                        Text = "現在購買享優惠，全品項 9 折",
        //                                        Actions = new List<ActionDto>
        //                                        {
        //                                            // 按鈕 action
        //                                            new ActionDto
        //                                            {
        //                                                Type = ActionTypeEnum.Uri,
        //                                                Label ="立即購買",
        //                                                Uri = "https://www.apple.com/tw/iphone-14-pro/?afid=p238%7Cs2W650oa9-dc_mtid_2092576n66464_pcrid_620529299490_pgrid_144614079327_&cid=wwa-tw-kwgo-iphone-slid---productid--Brand-iPhone14Pro-Announce-"
        //                                            }
        //                                        }
        //                                    },
        //                                }
        //                            }
        //                        }
        //                    }
        //                };
        //            }
        //            // 關鍵字 : "ImageCarousel"
        //            if(eventDto.Message.Text == "ImageCarousel")
        //            {
        //                replyMessage = new ReplyMessageRequestDto<TemplateMessageDto<ImageCarouselTemplateDto>>
        //                {
        //                    ReplyToken = eventDto.ReplyToken,
        //                    Messages = new List<TemplateMessageDto<ImageCarouselTemplateDto>>
        //                    {
        //                        new TemplateMessageDto<ImageCarouselTemplateDto>
        //                        {
        //                            AltText = "這是圖片輪播訊息",
        //                            Template = new ImageCarouselTemplateDto
        //                            {
        //                                Columns = new List<ImageCarouselColumnObjectDto>
        //                                {
        //                                    new ImageCarouselColumnObjectDto
        //                                    {
        //                                        ImageUrl = "https://i.imgur.com/74I9rlb.png",
        //                                        Action = new ActionDto
        //                                        {
        //                                            Type = ActionTypeEnum.Uri,
        //                                            Label = "前往官網",
        //                                            Uri = "https://www.apple.com/tw/iphone-14-pro/?afid=p238%7Cs2W650oa9-dc_mtid_2092576n66464_pcrid_620529299490_pgrid_144614079327_&cid=wwa-tw-kwgo-iphone-slid---productid--Brand-iPhone14Pro-Announce-"
        //                                        }
        //                                    },
        //                                    new ImageCarouselColumnObjectDto
        //                                    {
        //                                        ImageUrl = "https://www.apple.com/v/iphone-14-pro/a/images/meta/iphone-14-pro_overview__e2a7u9jy63ma_og.png",
        //                                        Action = new ActionDto
        //                                        {
        //                                            Type = ActionTypeEnum.Uri,
        //                                            Label = "立即購買",
        //                                            Uri = "https://www.apple.com/tw/iphone-14-pro/?afid=p238%7Cs2W650oa9-dc_mtid_2092576n66464_pcrid_620529299490_pgrid_144614079327_&cid=wwa-tw-kwgo-iphone-slid---productid--Brand-iPhone14Pro-Announce-"
        //                                        }
        //                                    },

        //                                }
        //                            }
        //                        }
        //                    }
        //                };
        //            }

        //            break;
        //    }

        //    ReplyMessageHandler(replyMessage);
        //}
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

