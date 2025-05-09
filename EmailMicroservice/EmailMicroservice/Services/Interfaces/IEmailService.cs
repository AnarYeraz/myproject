﻿namespace EmailMicroservice.Services.Interfaces
{
    public interface IEmailService
    {
        public Task<(bool Success, string? message)> SendEmailAsync(string email, string message);
    }
}
