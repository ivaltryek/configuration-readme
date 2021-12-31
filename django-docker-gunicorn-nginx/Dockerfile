FROM python:3.8.5-alpine

RUN pip install --upgrade pip

WORKDIR /app

COPY . .
RUN pip install -r requirements.txt

CMD gunicorn --preload mysite.wsgi:application --bind 0.0.0.0:8000