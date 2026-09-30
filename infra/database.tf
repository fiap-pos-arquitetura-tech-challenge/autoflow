resource "aws_security_group" "rds" {
  name        = "${var.project_name}-rds-sg"
  description = "Security Group para o RDS SQL Server"
  vpc_id      = aws_vpc.autoflow.id

  ingress {
    description = "SQL Server dentro da VPC"
    from_port   = 1433
    to_port     = 1433
    protocol    = "tcp"
    cidr_blocks = [aws_vpc.autoflow.cidr_block]
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = {
    Name    = "${var.project_name}-rds-sg"
    Project = var.project_name
  }
}

resource "aws_db_subnet_group" "autoflow" {
  name = "${var.project_name}-db-subnet-group"

  subnet_ids = [
    aws_subnet.private_a.id,
    aws_subnet.private_b.id
  ]

  tags = {
    Name    = "${var.project_name}-db-subnet-group"
    Project = var.project_name
  }
}

resource "aws_db_instance" "autoflow" {
  identifier = "${var.project_name}-sqlserver"

  engine         = "sqlserver-ex"
  instance_class = "db.t3.micro"

  allocated_storage = 20
  storage_type      = "gp3"

  username = var.db_username
  password = var.db_password

  port = 1433

  db_subnet_group_name   = aws_db_subnet_group.autoflow.name
  vpc_security_group_ids = [aws_security_group.rds.id]

  publicly_accessible = false
  multi_az            = false

  skip_final_snapshot = true

  tags = {
    Name    = "${var.project_name}-sqlserver"
    Project = var.project_name
  }
}