using System.Net.Http.Headers;
using LineBotMessage.Dtos;
using LineBotMessage.Enum;
using LineBotMessage.Providers;
using System.Text;

namespace LineBotMessage.Domain
{
    public class LineBotService
    {

        // 貼上 messaging api channel 中的 accessToken & secret
        private readonly string channelAccessToken = "gCEru16JH8CSHv+YoIXiCDD+vac9RAiIr/eJaXL4ZbRaRhwJdpJa8Uhd59DoXAjAXEvXYXbTCnIScSxl7ek2S/rV4LHBaxXt4I4bgSsuWM0gu9vncuOxFZ9odba9x7J0+P7j9ioVFweZe/Dhfq8fcwdB04t89/1O/w1cDnyilFU=";
        private readonly string channelSecret = "7b79ab80c255e148755672de6e73583b";

        private readonly string replyMessageUri = "https://api.line.me/v2/bot/message/reply";
        private readonly string broadcastMessageUri = "https://api.line.me/v2/bot/message/broadcast";


        private static HttpClient client = new HttpClient();
        private readonly JsonProvider _jsonProvider = new JsonProvider();

        public LineBotService() { }

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
                    case WebhookEventTypeEnum.Unsend:
                        Console.WriteLine($"使用者{eventObject.Source.UserId}在聊天室收回訊息！");
                        break;
                    case WebhookEventTypeEnum.Follow:
                        Console.WriteLine($"使用者{eventObject.Source.UserId}將我們新增為好友！");
                        break;
                    case WebhookEventTypeEnum.Unfollow:
                        Console.WriteLine($"使用者{eventObject.Source.UserId}封鎖了我們！");
                        break;
                    case WebhookEventTypeEnum.Join:
                        Console.WriteLine("我們被邀請進入聊天室了！");
                        break;
                    case WebhookEventTypeEnum.Leave:
                        Console.WriteLine("我們被聊天室踢出了");
                        break;
                    case WebhookEventTypeEnum.MemberJoined:
                        string joinedMemberIds = "";
                        foreach (var member in eventObject.Joined.Members)
                        {
                            joinedMemberIds += $"{member.UserId} ";
                        }
                        Console.WriteLine($"使用者{joinedMemberIds}加入了群組！");
                        break;
                    case WebhookEventTypeEnum.MemberLeft:
                        string leftMemberIds = "";
                        foreach (var member in eventObject.Left.Members)
                        {
                            leftMemberIds += $"{member.UserId} ";
                        }
                        Console.WriteLine($"使用者{leftMemberIds}離開了群組！");
                        break;
                    case WebhookEventTypeEnum.Postback:
                        Console.WriteLine($"使用者{eventObject.Source.UserId}觸發了postback事件");
                        break;
                    case WebhookEventTypeEnum.VideoPlayComplete:
                        replyMessage = new ReplyMessageRequestDto<TextMessageDto>()
                        {
                            ReplyToken = eventObject.ReplyToken,
                            Messages = new List<TextMessageDto>
                            {
                                new TextMessageDto(){Text = $"使用者您好，謝謝您收看我們的宣傳影片，祝您身體健康萬事如意 !"}
                            }
                        };
                        ReplyMessageHandler(replyMessage);
                        break;
                }
            }
        }

        private void ReceiveMessageWebhookEvent(WebhookEventDto eventDto)
        {
            dynamic replyMessage = new ReplyMessageRequestDto<BaseMessageDto>();

            switch (eventDto.Message.Type)
            {
                // 收到文字訊息
                case MessageTypeEnum.Text:
                    // 關鍵字 : "測試"
                    if (eventDto.Message.Text == "測試")
                    {
                        // 回覆文字訊息並挾帶 quick reply button
                        replyMessage = new ReplyMessageRequestDto<TextMessageDto>
                        {
                            ReplyToken = eventDto.ReplyToken,
                            Messages = new List<TextMessageDto>
                            {
                                new TextMessageDto
                                {
                                    Text = "QuickReply 測試訊息",
                                    QuickReply = new QuickReplyItemDto
                                    {
                                        Items = new List<QuickReplyButtonDto>
                                        {
                                            // message action
                                            new QuickReplyButtonDto {
                                                Action = new ActionDto {
                                                    Type = ActionTypeEnum.Message,
                                                    Label = "message 測試" ,
                                                    Text = "測試"
                                                }
                                            },
                                            // uri action
                                            new QuickReplyButtonDto {
                                                Action = new ActionDto {
                                                    Type = ActionTypeEnum.Uri,
                                                    Label = "uri 測試" ,
                                                    Uri = "https://www.appx.com.tw"
                                                }
                                            },
                                             // 使用 uri schema 分享 Line Bot 資訊
                                            new QuickReplyButtonDto {
                                                Action = new ActionDto {
                                                    Type = ActionTypeEnum.Uri,
                                                    Label = "分享 Line Bot 資訊" ,
                                                    Uri = "https://line.me/R/nv/recommendOA/@089yvykp"
                                                }
                                            },
                                            // postback action
                                            new QuickReplyButtonDto {
                                                Action = new ActionDto {
                                                    Type = ActionTypeEnum.Postback,
                                                    Label = "postback 測試" ,
                                                    Data = "quick reply postback action" ,
                                                    DisplayText = "使用者傳送 displayTex，但不會有 Webhook event 產生。",
                                                    InputOption = PostbackInputOptionEnum.OpenKeyboard,
                                                    FillInText = "第一行\n第二行"
                                                }
                                            },
                                            // datetime picker action
                                            new QuickReplyButtonDto {
                                                Action = new ActionDto {
                                                Type = ActionTypeEnum.DatetimePicker,
                                                Label = "日期時間選擇",
                                                    Data = "quick reply datetime picker action",
                                                    Mode = DatetimePickerModeEnum.Datetime,
                                                    Initial = "2022-09-30T19:00",
                                                    Max = "2022-12-31T23:59",
                                                    Min = "2021-01-01T00:00"
                                                }
                                            },
                                            // camera action
                                            new QuickReplyButtonDto {
                                                Action = new ActionDto {
                                                    Type = ActionTypeEnum.Camera,
                                                    Label = "開啟相機"
                                                }
                                            },
                                            // camera roll action
                                            new QuickReplyButtonDto {
                                                Action = new ActionDto {
                                                    Type = ActionTypeEnum.CameraRoll,
                                                    Label = "開啟相簿"
                                                }
                                            },
                                            // location action
                                            new QuickReplyButtonDto {
                                                Action = new ActionDto {
                                                    Type = ActionTypeEnum.Location,
                                                    Label = "開啟位置"
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        };
                    }
                    // 關鍵字 : "Sender"
                    if (eventDto.Message.Text == "Sender")
                    {
                        replyMessage = new ReplyMessageRequestDto<TextMessageDto>
                        {
                            ReplyToken = eventDto.ReplyToken,
                            Messages = new List<TextMessageDto>
                            {
                                new TextMessageDto
                                {
                                    Text = "你好，我是客服人員 1號",
                                    Sender = new SenderDto
                                    {
                                        Name = "客服人員 1號",
                                        IconUrl = "https://b489-61-63-154-173.jp.ngrok.io/UploadFiles/man.png"
                                    }
                                },
                                new TextMessageDto
                                {
                                    Text = "你好，我是客服人員 2號",
                                    Sender = new SenderDto
                                    {
                                        Name = "客服人員 2號",
                                        IconUrl = "https://b489-61-63-154-173.jp.ngrok.io/UploadFiles/gamer.png"
                                    }
                                }
                            }
                        };
                    }
                    // 關鍵字 : "Buttons"
                    if (eventDto.Message.Text == "Buttons")
                    {
                        replyMessage = new ReplyMessageRequestDto<TemplateMessageDto<ButtonsTemplateDto>>
                        {
                            ReplyToken = eventDto.ReplyToken,
                            Messages = new List<TemplateMessageDto<ButtonsTemplateDto>>
                            {
                                new TemplateMessageDto<ButtonsTemplateDto>
                                {
                                    AltText = "這是按鈕模板訊息",
                                    Template = new ButtonsTemplateDto
                                    {
                                        ThumbnailImageUrl = "https://i.imgur.com/CP6ywwc.jpg",
                                        ImageAspectRatio = TemplateImageAspectRatioEnum.Rectangle,
                                        ImageSize = TemplateImageSizeEnum.Cover,
                                        Title = "親愛的用戶您好，歡迎您使用本美食推薦系統",
                                        Text = "請選擇今天想吃的主食，系統會推薦附近的餐廳給您。",
                                        Actions = new List<ActionDto>
                                        {
                                            new ActionDto
                                            {
                                                Type = ActionTypeEnum.Postback,
                                                Data = "foodType=sushi",
                                                Label = "壽司",
                                                DisplayText = "壽司"
                                            },
                                            new ActionDto
                                            {
                                                Type = ActionTypeEnum.Postback,
                                                Data = "foodType=hot-pot",
                                                Label = "火鍋",
                                                DisplayText = "火鍋"
                                            },
                                            new ActionDto
                                            {
                                                Type = ActionTypeEnum.Postback,
                                                Data = "foodType=steak",
                                                Label = "牛排",
                                                DisplayText = "牛排"
                                            },
                                            new ActionDto
                                            {
                                                Type = ActionTypeEnum.Postback,
                                                Data = "foodType=next",
                                                Label = "下一個",
                                                DisplayText = "下一個"
                                            }
                                        }
                                    }
                                }
                            }
                        };
                    }
                    // 關鍵字 : "Confirm"
                    if (eventDto.Message.Text == "Confirm")
                    {
                        replyMessage = new ReplyMessageRequestDto<TemplateMessageDto<ConfirmTemplateDto>>
                        {
                            ReplyToken = eventDto.ReplyToken,
                            Messages = new List<TemplateMessageDto<ConfirmTemplateDto>>
                            {
                                new TemplateMessageDto<ConfirmTemplateDto>
                                {
                                    AltText = "這是確認模組訊息",
                                    Template = new ConfirmTemplateDto
                                    {
                                        Text = "請問您是否喜歡本產品?\n(產品編號123)",
                                        Actions = new List<ActionDto>
                                        {
                                            new ActionDto
                                            {
                                                Type = ActionTypeEnum.Postback,
                                                Data = "id=123&like=yes",
                                                Label = "喜歡",
                                                DisplayText = "喜歡",
                                            },
                                            new ActionDto
                                            {
                                                Type = ActionTypeEnum.Postback,
                                                Data = "id=123&like=no",
                                                Label = "不喜歡",
                                                DisplayText = "不喜歡",
                                            }
                                        }

                                    }
                                }

                            }
                        };
                    }
                    // 關鍵字 : "Carousel"
                    if(eventDto.Message.Text == "Carousel")
                    {
                        replyMessage = new ReplyMessageRequestDto<TemplateMessageDto<CarouselTemplateDto>>
                        {
                            ReplyToken = eventDto.ReplyToken,
                            Messages = new List<TemplateMessageDto<CarouselTemplateDto>>
                            {
                                new TemplateMessageDto<CarouselTemplateDto>
                                {
                                    AltText = "這是輪播訊息",
                                    Template = new CarouselTemplateDto
                                    {
                                        Columns = new List<CarouselColumnObjectDto>
                                        {
                                            //column objects
                                            new CarouselColumnObjectDto
                                            {
                                                ThumbnailImageUrl = "https://www.apple.com/v/iphone-14-pro/a/images/meta/iphone-14-pro_overview__e2a7u9jy63ma_og.png",
                                                Title = "全新上市 iPhone 14 Pro",
                                                Text = "現在購買享優惠，全品項 9 折",
                                                Actions = new List<ActionDto>
                                                {
                                                    // 按鈕 action
                                                    new ActionDto
                                                    {
                                                        Type = ActionTypeEnum.Uri,
                                                        Label ="立即購買",
                                                        Uri = "https://www.apple.com/tw/iphone-14-pro/?afid=p238%7Cs2W650oa9-dc_mtid_2092576n66464_pcrid_620529299490_pgrid_144614079327_&cid=wwa-tw-kwgo-iphone-slid---productid--Brand-iPhone14Pro-Announce-"
                                                    }
                                                }
                                            },
                                        }
                                    }
                                }
                            }
                        };
                    }
                    // 關鍵字 : "ImageCarousel"
                    if(eventDto.Message.Text == "ImageCarousel")
                    {
                        replyMessage = new ReplyMessageRequestDto<TemplateMessageDto<ImageCarouselTemplateDto>>
                        {
                            ReplyToken = eventDto.ReplyToken,
                            Messages = new List<TemplateMessageDto<ImageCarouselTemplateDto>>
                            {
                                new TemplateMessageDto<ImageCarouselTemplateDto>
                                {
                                    AltText = "這是圖片輪播訊息",
                                    Template = new ImageCarouselTemplateDto
                                    {
                                        Columns = new List<ImageCarouselColumnObjectDto>
                                        {
                                            new ImageCarouselColumnObjectDto
                                            {
                                                ImageUrl = "https://i.imgur.com/74I9rlb.png",
                                                Action = new ActionDto
                                                {
                                                    Type = ActionTypeEnum.Uri,
                                                    Label = "前往官網",
                                                    Uri = "https://www.apple.com/tw/iphone-14-pro/?afid=p238%7Cs2W650oa9-dc_mtid_2092576n66464_pcrid_620529299490_pgrid_144614079327_&cid=wwa-tw-kwgo-iphone-slid---productid--Brand-iPhone14Pro-Announce-"
                                                }
                                            },
                                            new ImageCarouselColumnObjectDto
                                            {
                                                ImageUrl = "https://www.apple.com/v/iphone-14-pro/a/images/meta/iphone-14-pro_overview__e2a7u9jy63ma_og.png",
                                                Action = new ActionDto
                                                {
                                                    Type = ActionTypeEnum.Uri,
                                                    Label = "立即購買",
                                                    Uri = "https://www.apple.com/tw/iphone-14-pro/?afid=p238%7Cs2W650oa9-dc_mtid_2092576n66464_pcrid_620529299490_pgrid_144614079327_&cid=wwa-tw-kwgo-iphone-slid---productid--Brand-iPhone14Pro-Announce-"
                                                }
                                            },
                                            
                                        }
                                    }
                                }
                            }
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

