DECLARE @Id int, @ClientId nvarchar(200), @AdminBaseUri nvarchar(1000), @Secret nvarchar(100), @RedirectUri1 nvarchar(2000), @RedirectUri2 nvarchar(2000)
DECLARE @IdentityResourceId int, @ApiScopeName nvarchar(1000), @ApiResourceId int, @ApiResourceName nvarchar(1000)
SET @ClientId = 'min.identity-server4.admin.vue'
SET @AdminBaseUri = 'http://localhost:9000'
SET @ApiScopeName = 'min.identity-server4.admin.api.scope'
SET @ApiResourceName = 'min.identity-server4.admin.api'

IF EXISTS(SELECT Id FROM IdentityResources WHERE ([Name] = 'openid'))
BEGIN
	SELECT @IdentityResourceId = Id FROM IdentityResources WHERE ([Name] = 'openid')
END
ELSE
BEGIN
	INSERT INTO IdentityResources
					(Enabled, [Name], DisplayName, Description, Required, Emphasize, ShowInDiscoveryDocument, Created, Updated, 
					NonEditable)
	VALUES   (1, 'openid', 'Your user identifier', NULL, 1, 0, 1, GETDATE(), NULL, 0)
	SELECT @IdentityResourceId = @@IDENTITY
END

IF EXISTS(SELECT Id FROM ApiScopes WHERE ([Name] = @ApiScopeName))
BEGIN
	UPDATE ApiScopes SET Enabled = 1 WHERE ([Name] = @ApiScopeName)
END
ELSE
BEGIN
	INSERT INTO ApiScopes
					(Enabled, [Name], DisplayName, Description, Required, Emphasize, ShowInDiscoveryDocument)
	VALUES   (1, @ApiScopeName, @ApiScopeName, NULL, 0, 0, 1)
END

IF EXISTS(SELECT Id FROM ApiResources WHERE ([Name] = @ApiResourceName))
BEGIN
	UPDATE ApiResources SET Enabled = 1 WHERE ([Name] = @ApiResourceName)
	SELECT @ApiResourceId = Id FROM ApiResources WHERE ([Name] = @ApiResourceName)
	IF NOT EXISTS(SELECT Id FROM ApiResourceScopes WHERE (Scope = @ApiScopeName))
	BEGIN
		INSERT INTO ApiResourceScopes (ApiResourceId, Scope)
		VALUES (@ApiResourceId, @ApiScopeName)
	END
END
ELSE
BEGIN
	INSERT INTO ApiResources
					(Enabled, [Name], DisplayName, Description, AllowedAccessTokenSigningAlgorithms, ShowInDiscoveryDocument, 
					Created, Updated, LastAccessed, NonEditable)
	VALUES   (1, @ApiResourceName, @ApiResourceName, NULL, NULL, 1, GETDATE(), NULL, NULL, 0)
	SET @ApiResourceId = @@IDENTITY
	INSERT INTO ApiResourceScopes(ApiResourceId, Scope)
	VALUES(@ApiResourceId, @ApiScopeName)
END

IF EXISTS(SELECT Id FROM Clients WHERE (ClientId = @ClientId))
BEGIN
	UPDATE  Clients
	SET         Enabled = 1, ClientId = @ClientId, ProtocolType = 'oidc', RequireClientSecret = 0, ClientName = 'IDS4 Admin Client', 
					Description = '', ClientUri = NULL, LogoUri = NULL, RequireConsent = 0, AllowRememberConsent = 1, 
					AlwaysIncludeUserClaimsInIdToken = 0, RequirePkce = 1, AllowPlainTextPkce = 0, RequireRequestObject = 0, 
					AllowAccessTokensViaBrowser = 0, FrontChannelLogoutUri = @AdminBaseUri + '/oidc/signout', 
					FrontChannelLogoutSessionRequired = 1, BackChannelLogoutUri = NULL, BackChannelLogoutSessionRequired = 1, 
					AllowOfflineAccess = 1, IdentityTokenLifetime = 300, AllowedIdentityTokenSigningAlgorithms = NULL, 
					AccessTokenLifetime = 3600, AuthorizationCodeLifetime = 300, ConsentLifetime = NULL, 
					AbsoluteRefreshTokenLifetime = 2592000, SlidingRefreshTokenLifetime = 1296000, RefreshTokenUsage = 0, 
					UpdateAccessTokenClaimsOnRefresh = 1, RefreshTokenExpiration = 1, AccessTokenType = 0, EnableLocalLogin = 1, 
					IncludeJwtId = 1, AlwaysSendClientClaims = 0, ClientClaimsPrefix = 'client_', PairWiseSubjectSalt = NULL, 
					Created = GETDATE(), Updated = NULL, LastAccessed = NULL, UserSsoLifetime = NULL, UserCodeType = NULL, 
					DeviceCodeLifetime = 300, NonEditable = 0
	WHERE   (ClientId = @ClientId)
	SELECT @Id = Id FROM Clients WHERE (ClientId = @ClientId)
END
ELSE
BEGIN
	INSERT INTO Clients
					(Enabled, ClientId, ProtocolType, RequireClientSecret, ClientName, Description, ClientUri, LogoUri, RequireConsent, 
					AllowRememberConsent, AlwaysIncludeUserClaimsInIdToken, RequirePkce, AllowPlainTextPkce, RequireRequestObject, 
					AllowAccessTokensViaBrowser, FrontChannelLogoutUri, FrontChannelLogoutSessionRequired, BackChannelLogoutUri, 
					BackChannelLogoutSessionRequired, AllowOfflineAccess, IdentityTokenLifetime, 
					AllowedIdentityTokenSigningAlgorithms, AccessTokenLifetime, AuthorizationCodeLifetime, ConsentLifetime, 
					AbsoluteRefreshTokenLifetime, SlidingRefreshTokenLifetime, RefreshTokenUsage, 
					UpdateAccessTokenClaimsOnRefresh, RefreshTokenExpiration, AccessTokenType, EnableLocalLogin, IncludeJwtId, 
					AlwaysSendClientClaims, ClientClaimsPrefix, PairWiseSubjectSalt, Created, Updated, LastAccessed, UserSsoLifetime, 
					UserCodeType, DeviceCodeLifetime, NonEditable)
	VALUES   (1, @ClientId, 'oidc', 0, 'IDS4 Admin Client', '', NULL, NULL, 0, 1, 0, 1, 0, 0, 0,@AdminBaseUri 
					+ '/oidc/signout', 1, NULL, 1, 1, 300, NULL, 3600, 300, NULL, 2592000, 1296000, 0, 1, 1, 0, 1, 1, 0, 'client_', NULL, 
					GETDATE(), NULL, NULL, NULL, NULL, 300, 0)
	SET @Id = @@IDENTITY
END

IF NOT EXISTS(SELECT Id FROM ClientGrantTypes WHERE (ClientId = @Id) AND (GrantType = 'authorization_code'))
BEGIN
	INSERT INTO ClientGrantTypes(ClientId, GrantType)
	VALUES(@Id, 'authorization_code')
END

IF NOT EXISTS(SELECT Id FROM ClientScopes WHERE (ClientId = @Id) AND (Scope = 'openid'))
BEGIN
	INSERT INTO ClientScopes(ClientId, Scope)
	VALUES(@Id, 'openid')
END

IF NOT EXISTS(SELECT Id FROM ClientScopes WHERE (ClientId = @Id) AND (Scope = @ApiScopeName))
BEGIN
	INSERT INTO ClientScopes(ClientId, Scope)
	VALUES(@Id, @ApiScopeName)
END

IF NOT EXISTS(SELECT Id FROM ClientRedirectUris WHERE (ClientId = @Id) AND (RedirectUri = @AdminBaseUri + '/oidc/signin'))
BEGIN
	INSERT INTO ClientRedirectUris(ClientId, RedirectUri)
	VALUES(@Id, @AdminBaseUri + '/oidc/signin')
END

IF NOT EXISTS(SELECT Id FROM ClientRedirectUris WHERE (ClientId = @Id) AND (RedirectUri = @AdminBaseUri + '/oidc/signin-silent'))
BEGIN
	INSERT INTO ClientRedirectUris(ClientId, RedirectUri)
	VALUES(@Id, @AdminBaseUri + '/oidc/signin-silent')
END

IF NOT EXISTS(SELECT Id FROM ClientPostLogoutRedirectUris WHERE (ClientId = @Id) AND (PostLogoutRedirectUri = @AdminBaseUri + '/oidc/signout-callback'))
BEGIN
	INSERT INTO ClientPostLogoutRedirectUris(ClientId, PostLogoutRedirectUri)
	VALUES(@Id, @AdminBaseUri + '/oidc/signout-callback')
END

IF NOT EXISTS(SELECT Id FROM ClientCorsOrigins WHERE (ClientId = @Id) AND (Origin = @AdminBaseUri))
BEGIN
	INSERT INTO ClientCorsOrigins(ClientId, Origin)
	VALUES(@Id, @AdminBaseUri)
END
