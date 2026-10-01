variable "aws_region" {
  description = "AWS Region"
  type        = string
  default     = "us-east-2"
}

variable "project_name" {
  description = "Project Name"
  type        = string
  default     = "autoflow"
}

variable "db_username" {
  description = "Usuario administrador do banco de dados"
  type        = string
  default     = "autoflowadmin"
}

variable "db_password" {
  description = "Senha do administrador do banco de dados"
  type        = string
  sensitive   = true
}