terraform {
  required_providers {
    azuread = {
      source  = "hashicorp/azuread"
      version = "2.40.0"
    }
    random = {
      source  = "hashicorp/random"
      version = "3.5.1"
    }
    time = {
      source  = "hashicorp/time"
      version = "0.9.1"
    }
  }
}


provider "azuread" {
  # Configuration options
}

provider "random" {
  # Configuration options
}

provider "time" {
  # Configuration options
}
