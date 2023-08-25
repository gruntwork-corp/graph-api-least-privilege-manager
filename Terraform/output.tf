output "config" {
  value = {
    ARM_CLIENT_ID     = azuread_application.this.application_id
    ARM_TENANT_ID     = azuread_service_principal.this.application_tenant_id
    ARM_CLIENT_SECRET = nonsensitive(azuread_application_password.this.value)
    IDENTIFIER_URI    = local.identifier_uri
  }
}

output "docker_compose_environment_config" {
  value = <<DOCKERCOMPOSECFG
- ARM_CLIENT_ID=${azuread_application.this.application_id}
- ARM_TENANT_ID=${azuread_service_principal.this.application_tenant_id}
- ARM_CLIENT_SECRET=${nonsensitive(azuread_application_password.this.value)}
- IDENTIFIER_URI=${local.identifier_uri}
    DOCKERCOMPOSECFG
}
