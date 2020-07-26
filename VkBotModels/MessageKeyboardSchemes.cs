using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.Keyboard;
using Newtonsoft.Json;

namespace WebBot
{
    static class MessageKeyboardSchemes
    {
        public static MessageKeyboard AllComandsButtonOnly = new MessageKeyboard
        {
            Buttons = new List<List<MessageKeyboardButton>>
            {
                new List<MessageKeyboardButton>()
                {
                    new MessageKeyboardButton
                    {
                        Action = new MessageKeyboardButtonAction
                        {
                            Type = KeyboardButtonActionType.Text,
                            Label = "Все команды",
                        },
                        Color = KeyboardButtonColor.Default
                    }
                }
            }
        };

        public static MessageKeyboard BackToMenuButtonOnly = new MessageKeyboard
        {
            Buttons = new List<List<MessageKeyboardButton>>
            {
                new List<MessageKeyboardButton>()
                {
                    new MessageKeyboardButton
                    {
                        Action = new MessageKeyboardButtonAction
                        {
                            Type = KeyboardButtonActionType.Text,
                            Label = "Меню",
                        },
                        Color = KeyboardButtonColor.Default
                    }
                }
            }
        };

        public static MessageKeyboard AskQuestion = new MessageKeyboard
        {
            Buttons = new List<List<MessageKeyboardButton>>
            {
                new List<MessageKeyboardButton>()
                {
                    new MessageKeyboardButton
                    {
                        Action = new MessageKeyboardButtonAction
                        {
                            Type = KeyboardButtonActionType.Text,
                            Label = "Спросить",
                        },
                        Color = KeyboardButtonColor.Primary
                    }
                },
                new List<MessageKeyboardButton>()
                {
                    new MessageKeyboardButton
                    {
                        Action = new MessageKeyboardButtonAction
                        {
                            Type = KeyboardButtonActionType.Text,
                            Label = "Меню",
                        },
                        Color = KeyboardButtonColor.Default
                    }
                }
            }
        };

        public static MessageKeyboard CancelOrderButtonOnly = new MessageKeyboard
        {
            Buttons = new List<List<MessageKeyboardButton>>
            {
                new List<MessageKeyboardButton>()
                {
                    new MessageKeyboardButton
                    {
                        Action = new MessageKeyboardButtonAction
                        {
                            Type = KeyboardButtonActionType.Text,
                            Label = "Отменить заказ",
                        },
                        Color = KeyboardButtonColor.Negative
                    }
                }
            }
        };

        public static MessageKeyboard PromoCodeChecking = new MessageKeyboard
        {
            Buttons = new List<List<MessageKeyboardButton>>
            {
                new List<MessageKeyboardButton>()
                {
                    new MessageKeyboardButton
                    {
                        Action = new MessageKeyboardButtonAction
                        {
                            Type = KeyboardButtonActionType.Text,
                            Label = "Назад",
                        },
                        Color = KeyboardButtonColor.Primary
                    }
                },
                new List<MessageKeyboardButton>()
                {
                    new MessageKeyboardButton
                    {
                        Action = new MessageKeyboardButtonAction
                        {
                            Type = KeyboardButtonActionType.Text,
                            Label = "Отменить заказ",
                        },
                        Color = KeyboardButtonColor.Negative
                    }
                }
            }
        };

        public static MessageKeyboard AddNewProductToOrder = new MessageKeyboard
        {
            Buttons = new List<List<MessageKeyboardButton>>
            {
                new List<MessageKeyboardButton>()
                {
                    new MessageKeyboardButton
                    {
                        Action = new MessageKeyboardButtonAction
                        {
                            Type = KeyboardButtonActionType.Text,
                            Label = "Продолжить",
                        },
                        Color = KeyboardButtonColor.Primary
                    }
                },
                new List<MessageKeyboardButton>()
                {
                    new MessageKeyboardButton
                    {
                        Action = new MessageKeyboardButtonAction
                        {
                            Type = KeyboardButtonActionType.Text,
                            Label = "Отменить заказ",
                        },
                        Color = KeyboardButtonColor.Negative
                    }
                }
            }
        };

        public static MessageKeyboard WriteToCustomerButtonOnly = new MessageKeyboard
        {
            Buttons = new List<List<MessageKeyboardButton>>
            {
                new List<MessageKeyboardButton>()
                {
                    new MessageKeyboardButton
                    {
                        Action = new MessageKeyboardButtonAction
                        {
                            Type = KeyboardButtonActionType.Text,
                            Label = "Написать покупателю",
                        },
                        Color = KeyboardButtonColor.Primary
                    }
                }
            }
        };

        public static MessageKeyboard OrderDefaultButtons = new MessageKeyboard
        {
            Buttons = new List<List<MessageKeyboardButton>>
            {
                new List<MessageKeyboardButton>()
                {
                    new MessageKeyboardButton
                    {
                        Action = new MessageKeyboardButtonAction
                        {
                            Type = KeyboardButtonActionType.Text,
                            Label = "Отменить заказ",
                        },
                        Color = KeyboardButtonColor.Negative
                    },
                    new MessageKeyboardButton
                    {
                        Action = new MessageKeyboardButtonAction
                        {
                            Type = KeyboardButtonActionType.Text,
                            Label = "Условия доставки",
                        },
                        Color = KeyboardButtonColor.Primary
                    }
                }
            }
        };

        public static MessageKeyboard ConfirmOrderWithPromo = new MessageKeyboard
        {
            Buttons = new List<List<MessageKeyboardButton>>
            {
                new List<MessageKeyboardButton>()
                {
                    new MessageKeyboardButton
                    {
                        Action = new MessageKeyboardButtonAction
                        {
                            Type = KeyboardButtonActionType.Text,
                            Label = "Подтвердить",
                        },
                        Color = KeyboardButtonColor.Positive
                    }
                },
                new List<MessageKeyboardButton>()
                {
                    new MessageKeyboardButton
                    {
                        Action = new MessageKeyboardButtonAction
                        {
                            Type = KeyboardButtonActionType.Text,
                            Label = "Отменить заказ",
                        },
                        Color = KeyboardButtonColor.Negative
                    },
                    new MessageKeyboardButton
                    {
                        Action = new MessageKeyboardButtonAction
                        {
                            Type = KeyboardButtonActionType.Text,
                            Label = "Промокод",
                        },
                        Color = KeyboardButtonColor.Primary
                    }
                }
            }
        };

        public static MessageKeyboard ConfirmOrderWithDeliveryInfo = new MessageKeyboard
        {
            Buttons = new List<List<MessageKeyboardButton>>
            {
                new List<MessageKeyboardButton>()
                {
                    new MessageKeyboardButton
                    {
                        Action = new MessageKeyboardButtonAction
                        {
                            Type = KeyboardButtonActionType.Text,
                            Label = "Подтвердить",
                        },
                        Color = KeyboardButtonColor.Positive
                    }
                },
                new List<MessageKeyboardButton>()
                {
                    new MessageKeyboardButton
                    {
                        Action = new MessageKeyboardButtonAction
                        {
                            Type = KeyboardButtonActionType.Text,
                            Label = "Отменить заказ",
                        },
                        Color = KeyboardButtonColor.Negative
                    },
                    new MessageKeyboardButton
                    {
                        Action = new MessageKeyboardButtonAction
                        {
                            Type = KeyboardButtonActionType.Text,
                            Label = "Условия доставки",
                        },
                        Color = KeyboardButtonColor.Primary
                    }
                }
            }
        };

        public static MessageKeyboard OnlyManualOrder = new MessageKeyboard
        {
            Buttons = new List<List<MessageKeyboardButton>>
            {
                new List<MessageKeyboardButton>()
                {
                    new MessageKeyboardButton
                    {
                        Action = new MessageKeyboardButtonAction
                        {
                            Type = KeyboardButtonActionType.Text,
                            Label = "Написать продавцу",
                        },
                        Color = KeyboardButtonColor.Primary
                    }
                },
                new List<MessageKeyboardButton>()
                {
                    new MessageKeyboardButton
                    {
                        Action = new MessageKeyboardButtonAction
                        {
                            Type = KeyboardButtonActionType.Text,
                            Label = "Отменить заказ",
                        },
                        Color = KeyboardButtonColor.Negative
                    },
                    new MessageKeyboardButton
                    {
                        Action = new MessageKeyboardButtonAction
                        {
                            Type = KeyboardButtonActionType.Text,
                            Label = "Условия доставки",
                        },
                        Color = KeyboardButtonColor.Default
                    }
                }
            }
        };

        public static MessageKeyboard SetLocation = new MessageKeyboard
        {
            Buttons = new List<List<MessageKeyboardButton>>
            {
                new List<MessageKeyboardButton>()
                {
                    new MessageKeyboardButton
                    {
                        Action = new MessageKeyboardButtonAction
                        {
                            Type = KeyboardButtonActionType.Location
                        }
                    }
                },
                new List<MessageKeyboardButton>()
                {
                    new MessageKeyboardButton
                    {
                        Action = new MessageKeyboardButtonAction
                        {
                            Type = KeyboardButtonActionType.Text,
                            Label = "Отменить заказ",
                        },
                        Color = KeyboardButtonColor.Negative
                    },
                    new MessageKeyboardButton
                    {
                        Action = new MessageKeyboardButtonAction
                        {
                            Type = KeyboardButtonActionType.Text,
                            Label = "Условия доставки",
                        },
                        Color = KeyboardButtonColor.Primary
                    }
                }
            }
        };

        public static MessageKeyboard DefaultButtons = new MessageKeyboard
        {
            Buttons = new List<List<MessageKeyboardButton>>
            {
                new List<MessageKeyboardButton>()
                {
                    new MessageKeyboardButton
                    {
                        Action = new MessageKeyboardButtonAction
                        {
                            Type = KeyboardButtonActionType.Text,
                            Label = "Сделать заказ",
                        },
                        Color = KeyboardButtonColor.Primary
                    }
                },
                new List<MessageKeyboardButton>()
                {
                    new MessageKeyboardButton
                    {
                        Action = new MessageKeyboardButtonAction
                        {
                            Type = KeyboardButtonActionType.Text,
                            Label = "Вопрос",
                        },
                        Color = KeyboardButtonColor.Default
                    },
                    new MessageKeyboardButton
                    {
                        Action = new MessageKeyboardButtonAction
                        {
                            Type = KeyboardButtonActionType.Text,
                            Label = "Отзывы",
                        },
                        Color = KeyboardButtonColor.Default
                    },
                    new MessageKeyboardButton
                    {
                        Action = new MessageKeyboardButtonAction
                        {
                            Type = KeyboardButtonActionType.Text,
                            Label = "Доставка",
                        },
                        Color = KeyboardButtonColor.Default
                    },
                    new MessageKeyboardButton
                    {
                        Action = new MessageKeyboardButtonAction
                        {
                            Type = KeyboardButtonActionType.Text,
                            Label = "Заказы",
                        },
                        Color = KeyboardButtonColor.Positive
                    }
                }
            }
        };

        public static MessageKeyboard PaymentMethodSelectionButtons = new MessageKeyboard
        {
            Buttons = new List<List<MessageKeyboardButton>>
            {
                new List<MessageKeyboardButton>()
                {
                    new MessageKeyboardButton
                    {
                        Action = new MessageKeyboardButtonAction
                        {
                            Type = KeyboardButtonActionType.Text,
                            Label = "Картой",
                        },
                        Color = KeyboardButtonColor.Primary
                    }
                },
                new List<MessageKeyboardButton>()
                {
                    new MessageKeyboardButton
                    {
                        Action = new MessageKeyboardButtonAction
                        {
                            Type = KeyboardButtonActionType.Text,
                            Label = "Наличными",
                        },
                        Color = KeyboardButtonColor.Positive
                    },
                },
                new List<MessageKeyboardButton>()
                {
                    new MessageKeyboardButton
                    {
                        Action = new MessageKeyboardButtonAction
                        {
                            Type = KeyboardButtonActionType.Text,
                            Label = "Отменить заказ",
                        },
                        Color = KeyboardButtonColor.Negative
                    },
                    new MessageKeyboardButton
                    {
                        Action = new MessageKeyboardButtonAction
                        {
                            Type = KeyboardButtonActionType.Text,
                            Label = "Условия доставки",
                        },
                        Color = KeyboardButtonColor.Primary
                    }
                }
            }
        };

        public static MessageKeyboard OrderModeSelectionButtons = new MessageKeyboard
        {
            Buttons = new List<List<MessageKeyboardButton>>
            {
                new List<MessageKeyboardButton>()
                {
                    new MessageKeyboardButton
                    {
                        Action = new MessageKeyboardButtonAction
                        {
                            Type = KeyboardButtonActionType.Text,
                            Label = "Автоматически",
                        },
                        Color = KeyboardButtonColor.Primary
                    }
                },
                new List<MessageKeyboardButton>()
                {
                    new MessageKeyboardButton
                    {
                        Action = new MessageKeyboardButtonAction
                        {
                            Type = KeyboardButtonActionType.Text,
                            Label = "Отменить заказ",
                        },
                        Color = KeyboardButtonColor.Negative
                    },
                    new MessageKeyboardButton
                    {
                        Action = new MessageKeyboardButtonAction
                        {
                            Type = KeyboardButtonActionType.Text,
                            Label = "Написать продавцу",
                        },
                        Color = KeyboardButtonColor.Default
                    }
                }
            }
        };
        public static MessageKeyboard AdminOrderConfirmationButtons = new MessageKeyboard
        {
            Buttons = new List<List<MessageKeyboardButton>>
            {
                new List<MessageKeyboardButton>()
                {
                    new MessageKeyboardButton
                    {
                        Action = new MessageKeyboardButtonAction
                        {
                            Type = KeyboardButtonActionType.Text,
                            Label = "Принять",
                        },
                        Color = KeyboardButtonColor.Positive
                    }
                },
                new List<MessageKeyboardButton>()
                {
                    new MessageKeyboardButton
                    {
                        Action = new MessageKeyboardButtonAction
                        {
                            Type = KeyboardButtonActionType.Text,
                            Label = "Отклонить",
                        },
                        Color = KeyboardButtonColor.Negative
                    },
                    new MessageKeyboardButton
                    {
                        Action = new MessageKeyboardButtonAction
                        {
                            Type = KeyboardButtonActionType.Text,
                            Label = "Таблица",
                        },
                        Color = KeyboardButtonColor.Primary
                    }
                }
            }
        };

        /// <summary>
        /// Убрать клавиатуру
        /// </summary>
        public static MessageKeyboard SetEmptyKeyboard = new MessageKeyboard
        {
            Buttons = new List<List<MessageKeyboardButton>>()
        };
    }
}
