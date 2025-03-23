using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace BallroomManager.Api.Filters
{
    public class SwaggerFileOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var actionDescriptor = context.ApiDescription.ActionDescriptor as ControllerActionDescriptor;
            if (actionDescriptor == null) return;

            if (actionDescriptor.ActionName == "CreateBallroom" || actionDescriptor.ActionName == "UpdateBallroom")
            {
                // Remove the automatically added parameter
                var fileParameter = operation.Parameters.FirstOrDefault(p => p.Name == "image");
                if (fileParameter != null)
                {
                    operation.Parameters.Remove(fileParameter);
                }

                // Ensure we have a request body
                if (operation.RequestBody == null)
                {
                    operation.RequestBody = new OpenApiRequestBody();
                }

                // Add multipart/form-data content type if not present
                if (!operation.RequestBody.Content.ContainsKey("multipart/form-data"))
                {
                    operation.RequestBody.Content["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = new Dictionary<string, OpenApiSchema>()
                        }
                    };
                }

                var schema = operation.RequestBody.Content["multipart/form-data"].Schema;

                // Add file property for image upload
                schema.Properties["image"] = new OpenApiSchema
                {
                    Type = "string",
                    Format = "binary",
                    Description = "The image file to upload"
                };

                // Add form fields for Ballroom properties
                schema.Properties["Name"] = new OpenApiSchema { Type = "string" };
                schema.Properties["Description"] = new OpenApiSchema { Type = "string" };
                schema.Properties["Dimesions"] = new OpenApiSchema { Type = "string" };
                schema.Properties["Capacity"] = new OpenApiSchema { Type = "integer", Format = "int32" };
                schema.Properties["IsAvailable"] = new OpenApiSchema { Type = "boolean" };
            }
        }
    }
} 