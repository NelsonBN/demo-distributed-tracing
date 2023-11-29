CREATE TABLE notification (
  id VARCHAR(36) PRIMARY KEY,
  version INT NOT NULL,
  message VARCHAR(500) NOT NULL,
  email VARCHAR(50) NULL,
  email_notification_status VARCHAR(10) NOT NULL,
  phone VARCHAR(50) NULL,
  phone_notification_status VARCHAR(10) NOT NULL);

