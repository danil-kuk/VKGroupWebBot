using System;
using System.Collections.Generic;
using System.Text;

namespace WebBot
{
    /// <summary>
    /// Этап оформления заказа
    /// </summary>
    public enum OrderStage
    {
        NoOrder,
        ProductSelection,
        OrderModeSelection,
        DeliveryAddressSelection,
        DeliveryDateTimeSelection,
        PaymentMethodSelection,
        PromoCodeChecking,
        DeliveryConfirmation
    }
}
