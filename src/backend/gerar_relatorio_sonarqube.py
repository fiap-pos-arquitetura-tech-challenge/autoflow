#!/usr/bin/env python3
"""
Gera um relatório HTML a partir da Web API do SonarQube.

Uso:
    python gerar_relatorio_sonarqube.py <project_key>

Exemplo:
    python gerar_relatorio_sonarqube.py minha-pos-api

Configuração (via variáveis de ambiente ou arquivo .env na pasta atual):
    SONAR_URL  (opcional) padrão: http://localhost:9000
    SONAR_TOKEN (opcional)
"""

import os
import sys
import html
from datetime import datetime
from pathlib import Path

import requests

try:
    from dotenv import load_dotenv

    load_dotenv()
except ImportError:
    pass


SONAR_URL = os.getenv("SONAR_URL", "http://localhost:9000").rstrip("/")
SONAR_TOKEN = os.getenv("SONAR_TOKEN", "")

print(f"Usando SONAR_URL={SONAR_URL}")
print(f"Usando SONAR_TOKEN={'***' if SONAR_TOKEN else '(não definido)'}")

METRICS = [
    "bugs",
    "vulnerabilities",
    "security_hotspots",
    "code_smells",
    "coverage",
    "duplicated_lines_density",
    "complexity",
    "ncloc",
    "sqale_index",
    "reliability_rating",
    "security_rating",
    "sqale_rating",
]

PAGE_SIZE = 500


def api_get(path, params=None):
    url = f"{SONAR_URL}{path}"
    kwargs = {"params": params or {}, "timeout": 30}

    if SONAR_TOKEN:
        kwargs["auth"] = (SONAR_TOKEN, "")

    response = requests.get(url, **kwargs)
    response.raise_for_status()
    return response.json()


def metric_value(measures, key):
    for measure in measures:
        if measure.get("metric") == key:
            return measure.get("value", "0")
    return "0"


def rating(value):
    ratings = {
        "1.0": "A",
        "2.0": "B",
        "3.0": "C",
        "4.0": "D",
        "5.0": "E",
    }
    return ratings.get(value, value)


def format_number(value):
    try:
        return f"{float(value):,.0f}".replace(",", ".")
    except (TypeError, ValueError):
        return value


def format_metric(key, value):
    if key == "coverage":
        return f"{value}%"
    if key == "duplicated_lines_density":
        return f"{value}%"
    if key == "complexity":
        return format_number(value)
    if key == "ncloc":
        return format_number(value)
    if key == "sqale_index":
        try:
            minutes = int(float(value))
            hours, mins = divmod(minutes, 60)
            if hours:
                return f"{hours}h {mins}min"
            return f"{mins}min"
        except ValueError:
            return value
    if key.endswith("_rating"):
        return rating(value)
    return format_number(value)


def fetch_issues(project_key):
    issues = []
    page = 1

    while True:
        data = api_get(
            "/api/issues/search",
            {
                "componentKeys": project_key,
                "ps": PAGE_SIZE,
                "p": page,
                "resolved": "false",
            },
        )

        batch = data.get("issues", [])
        issues.extend(batch)

        paging = data.get("paging", {})
        total = paging.get("total", len(issues))

        if not batch or len(issues) >= total:
            break

        page += 1

    return issues


def severity_class(severity):
    return {
        "BLOCKER": "blocker",
        "CRITICAL": "critical",
        "MAJOR": "major",
        "MINOR": "minor",
        "INFO": "info",
    }.get(severity, "")


def main():
    if len(sys.argv) != 2:
        print("Uso: python gerar_relatorio_sonarqube.py <project_key>")
        sys.exit(1)

    project_key = sys.argv[1]

    try:
        project = api_get(
            "/api/components/show",
            {"component": project_key},
        )["component"]

        measures = api_get(
            "/api/measures/component",
            {
                "component": project_key,
                "metricKeys": ",".join(METRICS),
            },
        )["component"]["measures"]

        quality_gate = api_get(
            "/api/qualitygates/project_status",
            {"projectKey": project_key},
        )["projectStatus"]

        issues = fetch_issues(project_key)

    except requests.exceptions.ConnectionError:
        print(
            f"Não foi possível conectar ao SonarQube em {SONAR_URL}.\n"
            "Verifique se o container está em execução e se a porta 9000 está exposta."
        )
        sys.exit(2)

    except requests.HTTPError as exc:
        print(f"Erro ao consultar a API do SonarQube: {exc}")
        if exc.response is not None:
            print(exc.response.text[:1000])
        sys.exit(3)

    generated_at = datetime.now().strftime("%d/%m/%Y %H:%M:%S")

    metrics_labels = {
        "bugs": "Bugs",
        "vulnerabilities": "Vulnerabilidades",
        "security_hotspots": "Security Hotspots",
        "code_smells": "Code Smells",
        "coverage": "Cobertura",
        "duplicated_lines_density": "Duplicação",
        "complexity": "Complexidade",
        "ncloc": "Linhas de código",
        "sqale_index": "Dívida técnica",
        "reliability_rating": "Reliability",
        "security_rating": "Security",
        "sqale_rating": "Maintainability",
    }

    cards = []
    for key in [
        "bugs",
        "vulnerabilities",
        "security_hotspots",
        "code_smells",
        "coverage",
        "duplicated_lines_density",
        "complexity",
        "ncloc",
    ]:
        value = format_metric(key, metric_value(measures, key))
        cards.append(
            f"""
            <div class="card">
                <div class="label">{html.escape(metrics_labels[key])}</div>
                <div class="value">{html.escape(str(value))}</div>
            </div>
            """
        )

    issue_rows = []
    for issue in issues:
        severity = html.escape(issue.get("severity", ""))
        message = html.escape(issue.get("message", ""))
        component = html.escape(
            issue.get("component", "").split(":", 1)[-1]
        )
        rule = html.escape(issue.get("rule", ""))
        issue_rows.append(
            f"""
            <tr>
                <td><span class="severity {severity_class(issue.get('severity'))}">
                    {severity}
                </span></td>
                <td>{message}</td>
                <td><code>{component}</code></td>
                <td>{rule}</td>
            </tr>
            """
        )

    issue_table = "\n".join(issue_rows) or """
        <tr><td colspan="4" class="empty">Nenhum issue aberto encontrado.</td></tr>
    """

    qg_status = quality_gate.get("status", "UNKNOWN")
    qg_class = "passed" if qg_status == "OK" else "failed"

    ratings_html = []
    for key in [
        "reliability_rating",
        "security_rating",
        "sqale_rating",
    ]:
        value = format_metric(key, metric_value(measures, key))
        ratings_html.append(
            f"""
            <div class="rating-card">
                <span>{html.escape(metrics_labels[key])}</span>
                <strong>{html.escape(str(value))}</strong>
            </div>
            """
        )

    html_report = f"""<!DOCTYPE html>
<html lang="pt-BR">
<head>
<meta charset="UTF-8">
<title>Relatório SonarQube - {html.escape(project.get("name", project_key))}</title>
<style>
    @page {{
        margin: 18mm;
    }}

    * {{
        box-sizing: border-box;
    }}

    body {{
        font-family: Arial, Helvetica, sans-serif;
        margin: 0;
        color: #202124;
        background: #f6f7f9;
    }}

    .container {{
        max-width: 1200px;
        margin: 0 auto;
        padding: 32px;
    }}

    header {{
        background: white;
        padding: 28px 32px;
        border-radius: 12px;
        margin-bottom: 22px;
        border: 1px solid #e2e5e9;
    }}

    h1 {{
        margin: 0 0 8px;
        font-size: 28px;
    }}

    h2 {{
        margin-top: 34px;
        font-size: 20px;
    }}

    .meta {{
        color: #667085;
        font-size: 14px;
    }}

    .quality {{
        margin-top: 20px;
        display: inline-block;
        padding: 9px 16px;
        border-radius: 20px;
        font-weight: bold;
    }}

    .passed {{
        background: #dff7e8;
        color: #16794c;
    }}

    .failed {{
        background: #fde2e1;
        color: #b42318;
    }}

    .cards {{
        display: grid;
        grid-template-columns: repeat(4, 1fr);
        gap: 14px;
    }}

    .card, .rating-card {{
        background: white;
        border: 1px solid #e2e5e9;
        border-radius: 10px;
        padding: 18px;
    }}

    .label {{
        color: #667085;
        font-size: 13px;
        margin-bottom: 8px;
    }}

    .value {{
        font-size: 25px;
        font-weight: bold;
    }}

    .ratings {{
        display: grid;
        grid-template-columns: repeat(3, 1fr);
        gap: 14px;
    }}

    .rating-card {{
        display: flex;
        justify-content: space-between;
        align-items: center;
    }}

    .rating-card span {{
        color: #667085;
    }}

    table {{
        width: 100%;
        border-collapse: collapse;
        background: white;
        border: 1px solid #e2e5e9;
        border-radius: 10px;
        overflow: hidden;
        font-size: 13px;
    }}

    th, td {{
        padding: 12px;
        border-bottom: 1px solid #eaecf0;
        text-align: left;
        vertical-align: top;
    }}

    th {{
        background: #f9fafb;
    }}

    code {{
        font-size: 12px;
    }}

    .severity {{
        font-weight: bold;
        font-size: 11px;
    }}

    .blocker, .critical {{
        color: #b42318;
    }}

    .major {{
        color: #b54708;
    }}

    .minor {{
        color: #667085;
    }}

    .info {{
        color: #175cd3;
    }}

    .empty {{
        text-align: center;
        padding: 30px;
        color: #667085;
    }}

    footer {{
        margin-top: 30px;
        color: #98a2b3;
        font-size: 12px;
        text-align: center;
    }}

    @media print {{
        body {{
            background: white;
        }}

        .container {{
            max-width: none;
            padding: 0;
        }}

        .card, .rating-card, header, table {{
            break-inside: avoid;
        }}
    }}

    @media (max-width: 800px) {{
        .cards {{
            grid-template-columns: repeat(2, 1fr);
        }}

        .ratings {{
            grid-template-columns: 1fr;
        }}
    }}
</style>
</head>

<body>
<div class="container">

<header>
    <h1>Relatório de Análise SonarQube</h1>
    <div class="meta">
        Projeto: <strong>{html.escape(project.get("name", project_key))}</strong><br>
        Project Key: <code>{html.escape(project_key)}</code><br>
        Gerado em: {generated_at}
    </div>

    <div class="quality {qg_class}">
        Quality Gate: {html.escape(qg_status)}
    </div>
</header>

<h2>Resumo</h2>

<div class="cards">
    {"".join(cards)}
</div>

<h2>Qualidade</h2>

<div class="ratings">
    {"".join(ratings_html)}
</div>

<h2>Issues encontrados</h2>

<table>
<thead>
<tr>
    <th>Severidade</th>
    <th>Mensagem</th>
    <th>Arquivo</th>
    <th>Regra</th>
</tr>
</thead>
<tbody>
{issue_table}
</tbody>
</table>

<footer>
    Relatório gerado automaticamente a partir da Web API do SonarQube.
</footer>

</div>
</body>
</html>
"""

    safe_name = "".join(
        c if c.isalnum() or c in "-_." else "_"
        for c in project_key
    )

    output = Path(f"relatorio-sonarqube-{safe_name}.html")
    output.write_text(html_report, encoding="utf-8")

    print(f"Relatório gerado: {output.resolve()}")


if __name__ == "__main__":
    main()
