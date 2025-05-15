namespace BulkySh.Utility
{
    public static class SD // Static Details
	{
        public static class Roles
        {
			public const string Customer = "Customer";
			public const string Company = "Company";
			public const string Admin = "Admin";
			public const string Employee = "Employee";
		}

		public static class Status
		{
			public const string Pending = "Pending";
			public const string Approved = "Approved";
			public const string InProcess = "Processing";
			public const string Shipped = "Shipped";
			public const string Cancelled = "Cancelled";
			public const string Refunded = "Refunded";
		}

		public static class PaymentStatus
		{
			public const string Pending = "Pending";
			public const string Approved = "Approved";
			public const string DelayedPayment = "ApprovedForDelayedPayment";
			public const string Rejected = "Rejected";
		}

		public const string SessionCart = "SessionShoppingCart";
	}
}