﻿using System.Collections.Generic;
using Digital_Content.DigitalContent.Editions;

namespace Digital_Content.DigitalContent.MultiTenancy.Payments.Dto
{
    public class ExecutePaymentDto
    {
        public SubscriptionPaymentGatewayType Gateway { get; set; }

        public EditionPaymentType EditionPaymentType { get; set; }

        public int EditionId { get; set; }

        public PaymentPeriodType PaymentPeriodType { get; set; }

        public Dictionary<string, string> AdditionalData { get; set; }

        public ExecutePaymentDto()
        {
            AdditionalData = new Dictionary<string, string>();
        }
    }
}
