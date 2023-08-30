data "azuread_client_config" "current" {}

data "azuread_application_published_app_ids" "well_known" {}

data "azuread_service_principal" "msgraph" {
  application_id = data.azuread_application_published_app_ids.well_known.result.MicrosoftGraph
}

resource "random_uuid" "scope" {}

resource "random_uuid" "roles" {
  for_each = toset(var.roles)
}

locals {
  identifier_uri                     = coalesce(var.identifier_uri, "api://${var.application_name}")
  authorization_consent_display_name = "API %s access"
  authorization_consent_description  = "Allow the application to access access the API with scope %s on your behalf."
}

resource "azuread_application" "this" {
  display_name = var.application_name
  #   description = "" /*(Optional) A description of the service principal provided for internal end-users.*/

  sign_in_audience = "AzureADMyOrg"

  feature_tags {
    /* Whether this application represents a custom SAML application for linked service principals */
    custom_single_sign_on = false
    hide                  = true //Whether this app is invisible to users in My Apps and Office 365 Launcher
    /*feature tag specifies that the application is an enterprise application. 
    This means that the application can be used by users in Azure Active Directory (Azure AD) tenant organizations. 
    The tag is required if you want to use the application for single sign-on
    */
    enterprise = true
    /*
    The gallery feature tag specifies that the application is available in the Azure AD gallery.
    When you enable the gallery feature tag, Azure AD will add the application to the Azure AD gallery. 
    This means that the application will be available for deployment to Azure AD tenant organizations.
    */
    gallery = false
  }

  identifier_uris = [
    local.identifier_uri
  ]

  required_resource_access {
    # resource_app_id = "00000003-0000-0000-c000-000000000000" //The AppId for Microsoft.Graph API
    resource_app_id = data.azuread_application_published_app_ids.well_known.result.MicrosoftGraph

    # type = "Role" //Application permission (DAEMON)      -> msgraph.app_role_ids["Group.ReadWrite.All"]
    # type = "Scope" //Delegated permission (on behalf of) -> msgraph.oauth2_permission_scope_ids["Group.ReadWrite.All"]
    resource_access {
      //Required to create/delete groups, as well as manage membership and and ownership
      type = "Role"
      id   = data.azuread_service_principal.msgraph.app_role_ids["Group.ReadWrite.All"]
    }
    resource_access {
      //Required to bind user principals as owners to groups
      type = "Role"
      id   = data.azuread_service_principal.msgraph.app_role_ids["User.Read.All"]
    }
    resource_access {
      //Required to bind service principals as owners to groups
      type = "Role"
      id   = data.azuread_service_principal.msgraph.app_role_ids["Application.Read.All"]
    }
  }

  web {
    implicit_grant {
      access_token_issuance_enabled = true
      id_token_issuance_enabled     = true
    }
  }

  single_page_application {
    redirect_uris = var.redirect_uris
  }

  api {
    mapped_claims_enabled          = true
    requested_access_token_version = 2
    oauth2_permission_scope {
      admin_consent_display_name = "Acquisition of OAuth token"
      admin_consent_description  = "Allow Acquisition of OAuth token for principals"
      enabled                    = true
      id                         = random_uuid.scope.result
      type                       = "User"
      user_consent_display_name  = "Acquisition of OAuth token"
      user_consent_description   = "Allow Acquisition of OAuth token for principals"
      value                      = "user_impersonation"
    }
  }

  dynamic "app_role" {
    for_each = toset(var.roles)

    content {
      allowed_member_types = ["Application", "User"]
      display_name         = format(local.authorization_consent_display_name, app_role.key)
      description          = format(local.authorization_consent_description, app_role.key)
      enabled              = true
      id                   = random_uuid.roles[app_role.key].result
      value                = app_role.key
    }
  }

  notes = jsonencode(var.tags)

  owners = concat([data.azuread_client_config.current.object_id], var.additional_owners)
}

resource "azuread_service_principal" "this" {
  application_id = azuread_application.this.application_id
  use_existing   = true

  app_role_assignment_required  = true
  preferred_single_sign_on_mode = "oidc"

  notes = jsonencode(var.tags)

  owners = concat([data.azuread_client_config.current.object_id], var.additional_owners)
}

resource "azuread_application_password" "this" {
  display_name          = "graph-api-least-privilege-manager-client-secret"
  application_object_id = azuread_application.this.object_id
}
