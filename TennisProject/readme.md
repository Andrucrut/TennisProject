### Для запуска на локалке:

1. Открыть ngrok -> ngrok http 8443 (ngrok start --all)
2. Получаем https ссылку Forwarding                    https://9a03-2001-41d0-306-3c0f-00.ngrok-free.app -> http://localhost:8443  
3. Сетим webhook https://api.telegram.org/bot{BOT_TOKEN}/setWebhook?url={HTTPS_NGROK_URL} 
   (пример: https://api.telegram.org/bot7127019597:AAFx5El7jBq6zuICXPIDBNYqD3jg9-HR1mE/setWebhook?url=https://webhook.unicort.ru
4. Меняем в appsetting.json "HostAddress" на HTTPS_NGROK_URL
5. Front-end:
   (TennisFrontend) npm run dev
   To do:
1. Логирование:
Логировать создание игры, изменение
Авторизацию, но потом ее убрать.

2. Create DTO for requests
3. Add mapper