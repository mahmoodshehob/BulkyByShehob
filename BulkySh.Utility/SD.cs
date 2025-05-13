namespace BulkySh.Utility
{
    public static class SD // Static Details
    {
        public const string Role_Customer = "Customer";
        public const string Role_Company = "Company";
        public const string Role_Admin = "Admin";
        public const string Role_Employee = "Employee";

    }

    public static class SD_Status
    {
        public const string Pending = "Pending";
        public const string Approved = "Approved";
        public const string InProcess = "Processing";
        public const string Shipped = "Shipped";
        public const string Cancelled = "Cancelled";
        public const string Refunded = "Refunded";
    }

    public static class SD_PaymentStatus
    {
        public const string Pending = "Pending";
        public const string Approved = "Approved";
        public const string DelayedPayment = "ApprovedForDelayedPayment";
        public const string Rejected = "Rejected";
    }
}