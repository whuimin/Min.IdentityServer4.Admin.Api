namespace Ids4AdminApi.Models
{
    /// <summary>
    ///  Defines result code for the API response.
    /// </summary>
    public enum ResultCode
    {
        /// <summary>
        /// System error
        /// </summary>
        SystemError,
        /// <summary>
        /// Parameter validation fail.
        /// </summary>
        ParameterInvalid,
        /// <summary>
        /// Data base error.
        /// </summary>
        DbError,
        /// <summary>
        /// Client is existed in database.
        /// </summary>
        ClientExist,
        /// <summary>
        /// Client is not exist in database.
        /// </summary>
        ClientNotExist,
        /// <summary>
        /// API scope is existed in database.
        /// </summary>
        ApiScopeExist,
        /// <summary>
        /// API scope is not exist in database.
        /// </summary>
        ApiScopeNotExist,
        /// <summary>
        /// API resource is existed in database.
        /// </summary>
        ApiResourceExist,
        /// <summary>
        /// API resource is not exist in database.
        /// </summary>
        ApiResourceNotExist,
        /// <summary>
        /// Identity resource is existed in database.
        /// </summary>
        IdentityResourceExist,
        /// <summary>
        /// Identity resource is not exist in database.
        /// </summary>
        IdentityResourceNotExist,
        /// <summary>
        /// Device flow code is existed in database.
        /// </summary>
        DeviceFlowCodeExist,
        /// <summary>
        /// Device flow code is not exist in database.
        /// </summary>
        DeviceFlowCodeNotExist,
        /// <summary>
        /// Persisted grant is existed in database.
        /// </summary>
        PersistedGrantExist,
        /// <summary>
        /// Persisted grant is not exist in database.
        /// </summary>
        PersistedGrantNotExist
    }
}
