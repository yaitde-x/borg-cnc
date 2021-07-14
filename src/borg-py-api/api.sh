#!/bin/sh
. venv/bin/activate
export FLASK_APP=api1.py
flask run --host=0.0.0.0 --port=6040
