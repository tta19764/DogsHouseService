﻿namespace DogsHouseService.WebApi.Options
{
    public class RateLimitingOptions
    {
        public int PermitLimit { get; set; }
        public int WindowSeconds { get; set; }
        public int QueueLimit { get; set; }
    }
}
