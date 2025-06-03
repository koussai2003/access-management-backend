using Microsoft.Extensions.Options;
using AccessManagementAPI.Models;
using AccessManagementAPI.Data;
using System.Threading.Tasks;
using System;

namespace AccessManagementAPI.Services
{
    public class EmailTemplateService
    {
        public string GetValidationEmail(string applicationName, string status, string validatorName = "")
        {
            return $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                <h2 style='color: #2c3e50;'>Access Request Update</h2>
                <p>Your request for <strong>{applicationName}</strong> has been updated.</p>
                <div style='background-color: #f8f9fa; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                    <p style='margin: 0;'><strong>Status:</strong> {status}</p>
                    {(string.IsNullOrEmpty(validatorName) ? "" : $"<p style='margin: 0;'><strong>Validator:</strong> {validatorName}</p>")}
                </div>
                <p>You can view the details in your Access Management account.</p>
                <div style='margin-top: 30px; padding-top: 20px; border-top: 1px solid #eee;'>
                    <p style='font-size: 0.9em; color: #7f8c8d;'>
                        This is an automated message. Please do not reply directly to this email.
                    </p>
                </div>
            </div>";
        }

        public string GetRejectionEmail(string applicationName, string comment, string rejectorName)
        {
            return $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                <h2 style='color: #e74c3c;'>Request Rejected</h2>
                <p>Your request for <strong>{applicationName}</strong> has been rejected.</p>
                <div style='background-color: #f8f9fa; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                    <p style='margin: 0;'><strong>Rejected by:</strong> {rejectorName}</p>
                    <p style='margin: 0;'><strong>Reason:</strong> {comment}</p>
                </div>
                <p>Please contact the administrator if you have any questions.</p>
                <div style='margin-top: 30px; padding-top: 20px; border-top: 1px solid #eee;'>
                    <p style='font-size: 0.9em; color: #7f8c8d;'>
                        This is an automated message. Please do not reply directly to this email.
                    </p>
                </div>
            </div>";
        }

        public string GetAcceptedEmail(string applicationName, string adminName, string nextSteps, DateTime? accessStartDate = null)
        {
            string accessDateInfo = accessStartDate.HasValue 
                ? $"<p>Your access will be active starting from <strong>{accessStartDate.Value.ToString("MMMM dd, yyyy")}</strong>.</p>"
                : "<p>Your access should now be active.</p>";

            return $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                <h2 style='color: #27ae60;'>Request Accepted</h2>
                <p>Your request for <strong>{applicationName}</strong> has been accepted and is being processed.</p>
        
            <div style='background-color: #f8f9fa; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                <p style='margin: 0;'><strong>Accepted by:</strong> {adminName}</p>
                <p style='margin: 10px 0 0;'><strong>Next steps:</strong> {nextSteps}</p>
            </div>
        
            {accessDateInfo}
        
            <p>You'll receive another notification once your access has been fully provisioned.</p>
        
            <div style='margin-top: 30px; padding-top: 20px; border-top: 1px solid #eee;'>
                <p style='font-size: 0.9em; color: #7f8c8d;'>
                    This is an automated message. Please do not reply directly to this email.
                </p>
            </div>
            </div>";
            }
        public string GetApprovalEmail(string applicationName, string adminName)
        {
            string accessDateInfo = "<p>Your access should now be active.</p>";

            return $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                <h2 style='color: #27ae60;'>Request Approved</h2>
                <p>Your request for <strong>{applicationName}</strong> has been approved.</p>
        
            <div style='background-color: #f8f9fa; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                <p style='margin: 0;'><strong>Approved by:</strong> {adminName}</p>
            </div>
        
            {accessDateInfo}
        
            <p>You're all set u have access now.</p>
        
            <div style='margin-top: 30px; padding-top: 20px; border-top: 1px solid #eee;'>
                <p style='font-size: 0.9em; color: #7f8c8d;'>
                    This is an automated message. Please do not reply directly to this email.
                </p>
            </div>
            </div>";
        }
    }
}
