namespace Ids4AdminApi.Models
{
    /// <summary>
    /// Represents a API response error.
    /// </summary>
    public class ResponseError
    {
        /// <summary>
        /// Gets or sets the error code of API response.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the error message of API response.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the parameter list of API response. It is used for format the error message.
        /// </summary>
        public string[] Params { get; set; }
    }
}
