# Задание 2

## Как бороться с этим автокликером

## Реализация динамического токена: 
Вместо использования статического авторизационного токена, который не изменяется, можно внедрить токены с ограниченным сроком действия (например, JWT с истечением через короткий промежуток времени).
Токены должны обновляться с каждым запросом, и их генерация должна зависеть от текущих данных пользователя, времени и уникальных идентификаторов сессии.
Это усложнит подмену запросов, так как злоумышленник не сможет повторно использовать один и тот же токен.

## Хеширование данных с секретным ключом:
Запросы с хешированными значениями, созданные на основе секретного ключа, который известен только серверу и клиенту. Например, каждый запрос может содержать хеш, основанный на текущем времени, данных пользователя и секретном ключе.
Сервер будет проверять соответствие этого хеша, и если он не совпадает, запрос будет отклонен.
Этот метод позволяет предотвращать отправку поддельных данных, так как злоумышленнику будет сложно правильно сгенерировать хеш без знания секретного ключа.

## Анализ шаблонов активности: 
реализация серверного анализа временных шаблонов и последовательностей щелчков для обнаружения неестественной активности. (Всегда идет sync запрос затем tap (но можно же добавить опять же 
случайны интервал времени между ними))
