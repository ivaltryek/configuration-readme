#!/bin/sh

gunicorn mysite.wsgi:application --bind 0.0.0.0:8000