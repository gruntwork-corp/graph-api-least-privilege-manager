variable "application_name" {
  type        = string
  description = "The display name of your application and service principal"
  default     = "graph-api-least-privilege-manager"
}

variable "identifier_uri" {
  type        = string
  description = "The identifier of the application in Azure AD. Will default to 'api://<var.application_name>'"
  default     = ""
}

variable "redirect_uris" {
  type    = list(string)
  description = "The pre-approved URIs Azure AD will send security tokens back after successfull authentication. Must be https (except for localhost). Must be unique. "
  default = ["http://localhost:5467/swagger/oauth2-redirect.html"]
}

variable "roles" {
  type = list(string)
  description = "The roles to create for the app. Must not container spaces or special characters other than '.'. For instance: 'Group.ReadWrite.OwnedBy'. Must be unique. "
  default = [
    "Group.Read",
    "Group.ReadWriteControl.OwnedBy",
    "Group.ReadWriteOwner.OwnedBy",
    "Group.ReadWriteMember.OwnedBy",
    "Group.ReadWrite.OwnedBy",
  ]
}

variable "tags" {
  type = map(string)
  description = "Key-value pairs of tags to assign to the app-registration and security service principal 'notes' field as a json-string"
  default = {
    "terraform" = "true"
    "project"   = "graph-api-least-privilege-manager"
  }
}

variable "additional_owners" {
  description = "List of object ids of additional users/service principals to assign as app-registration and security service principal owners. The caller will be added as owner by default."
  type        = list(string)
  default     = []
}
