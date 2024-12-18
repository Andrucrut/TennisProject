using Infrastructure.Data.Entities;
using Models.Models.Scrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Helpers
{
    public static class EnumConvertetr
    {
        public static BookingStatus ScrapperPaymentToBookingStatus(ScrapperPaymentStatus scrapperPaymentStatus)
        {
            switch (scrapperPaymentStatus)
            {
                case ScrapperPaymentStatus.Pending:
                    return BookingStatus.Pending;
                case ScrapperPaymentStatus.Declined:
                    return BookingStatus.NotPayedCancelled;
                case ScrapperPaymentStatus.Accepted:
                    return BookingStatus.Booked;
                case ScrapperPaymentStatus.Refunded:
                    return BookingStatus.Refaunded;
                case ScrapperPaymentStatus.Unknown:
                    return BookingStatus.Cancelled;
                default:
                    return BookingStatus.Pending; // Возвращаем Pending для Unknown и других неизвестных статусов
            }
        }

        public static BookingStatus AcquiringStatusToBookingStatus(string status)
        {
            status = status.ToLower();
            switch (status)
            {
                case "pending":
                    return BookingStatus.Pending;
                case "succeeded":
                    return BookingStatus.Booked;
                case "canceled":
                    return BookingStatus.Cancelled;
                case "waitingforcapture":
                    return BookingStatus.Booked;
                default:
                    return BookingStatus.Pending; // Возвращаем Pending для Unknown и других неизвестных статусов
            }
        }
    }
}
