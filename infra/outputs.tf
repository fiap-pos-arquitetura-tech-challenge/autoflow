output "ecr_repository_url" {
  value = data.aws_ecr_repository.autoflow.repository_url
}

output "rds_endpoint" {
  description = "Endpoint do RDS SQL Server"
  value       = aws_db_instance.autoflow.address
}

output "rds_port" {
  description = "Porta do RDS SQL Server"
  value       = aws_db_instance.autoflow.port
}