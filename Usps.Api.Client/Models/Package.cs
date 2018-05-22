using System;

namespace Usps.Api.Client.Models
{
    public class Package
    {
        public Package()
        {
            LabelType = LabelType.FullLabel;
            LabelImageType = LabelImageType.TIF;
            ServiceType = ServiceType.First_Class;
        }

        public LabelType LabelType { get; set; }

        public Address FromAddress { get; set; } = new Address();

        public Address ToAddress { get; set; } = new Address();

        public int WeightInOunces { get; set; } = 0;

        public ServiceType ServiceType { get; set; }

        public bool SeparateReceiptPage { get; set; } = false;

        public string OriginZipcode { get; set; } = "";

        public LabelImageType LabelImageType { get; set; } = LabelImageType.TIF;

        public DateTime ShipDate { get; set; } = DateTime.Now;

        public string ReferenceNumber { get; set; } = "";

        public bool AddressServiceRequested { get; set; } = false;

        public byte[] ShippingLabel { get; set; }

        public PackageType PackageType { get; set; }

        public PackageSize PackageSize { get; set; }
    }

    public enum PackageType
    {
        None,
        Flat_Rate_Envelope,
        Flat_Rate_Box
    }

    public enum PackageSize
    {
        None,
        Regular,
        Large,
        Oversize
    }

    public enum LabelImageType
    {
        TIF,
        PDF,
        None
    }

    public enum ServiceType
    {
        Priority,
        First_Class,
        Parcel_Post,
        Bound_Printed_Matter,
        Media_Mail,
        Library_Mail
    }

    public enum LabelType
    {
        FullLabel = 1,
        DeliveryConfirmationBarcode = 2
    }
}