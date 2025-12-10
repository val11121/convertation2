import { useState, useEffect } from "react";
import axios from "axios";
import { format } from "date-fns";
import "./App.css";

const API_URL = "https://localhost:7057/api/conversions"; // Работает через Vite proxy (или замени на полный URL)

interface Conversion {
  id: number;
  fromCurrency: string;
  toCurrency: string;
  amount: number;
  result: number;
  exchangeRate: number;
  timestamp: string;
}

interface RatesResponse {
  baseCurrency: string;
  note: string;
  updatedAt: string;
  rates: Record<string, number>;
}

function App() {
  const [amount, setAmount] = useState("100");
  const [from, setFrom] = useState("USD");
  const [to, setTo] = useState("RUB");
  const [result, setResult] = useState<Conversion | null>(null);
  const [history, setHistory] = useState<Conversion[]>([]);
  const [rates, setRates] = useState<Record<string, number>>({});
  const [updatedAt, setUpdatedAt] = useState<string>("");
  const [loading, setLoading] = useState({ convert: false, rates: false, history: false });
  const [error, setError] = useState("");

  const availableCurrencies = Object.keys(rates).length > 0
    ? Object.keys(rates).sort()
    : ["USD", "EUR", "GBP", "JPY", "CNY", "RUB", "CAD", "AUD", "CHF", "BYN", "INR"];

  // Загрузка данных
  const loadHistory = async () => {
    setLoading(prev => ({ ...prev, history: true }));
    setError("");
    try {
      const res = await axios.get<Conversion[]>(API_URL);
      setHistory(res.data.reverse());
    } catch {
      setError("Не удалось загрузить историю");
    } finally {
      setLoading(prev => ({ ...prev, history: false }));
    }
  };

  const loadRates = async () => {
    setLoading(prev => ({ ...prev, rates: true }));
    setError("");
    try {
      const res = await axios.get<RatesResponse>(`${API_URL}/rates`);
      setRates(res.data.rates);
      setUpdatedAt(res.data.updatedAt);
    } catch {
      setError("Не удалось загрузить курсы валют");
    } finally {
      setLoading(prev => ({ ...prev, rates: false }));
    }
  };

  const handleConvert = async () => {
    setError("");
    setResult(null);

    const numAmount = Number(amount);
    if (!amount || isNaN(numAmount) || numAmount <= 0) {
      setError("Введите корректную сумму");
      return;
    }

    setLoading(prev => ({ ...prev, convert: true }));
    try {
      const res = await axios.post<Conversion>(API_URL, {
        fromCurrency: from,
        toCurrency: to,
        amount: numAmount,
      });
      setResult(res.data);
      loadHistory(); // автообновление истории
    } catch (err: any) {
      const msg = err.response?.data || err.message;
      if (err.response?.status >= 500) setError("Сервис курсов недоступен");
      else if (err.response?.status === 400) setError(msg);
      else setError("Ошибка конвертации");
    } finally {
      setLoading(prev => ({ ...prev, convert: false }));
    }
  };

  useEffect(() => {
    loadRates();
    loadHistory();
  }, []);

  return (
    <div className="app-container">
      <div className="max-w-5xl mx-auto">
        <header className="header">
          <h1>Конвертер валют</h1>
          <p className="subtitle">Базовая валюта — RUB (реальные курсы в реальном времени)</p>
        </header>

        {/* Конвертер */}
        <section className="card converter-card">
          <h2>Конвертация</h2>

          <div className="converter-grid">
            <div>
              <label>Сумма</label>
              <input
                type="number"
                value={amount}
                onChange={(e) => setAmount(e.target.value)}
                placeholder="100"
              />
            </div>

            <div>
              <label>Из</label>
              <select value={from} onChange={(e) => setFrom(e.target.value)}>
                {availableCurrencies.map(c => (
                  <option key={c} value={c}>{c}</option>
                ))}
              </select>
            </div>

            <div>
              <label>В</label>
              <select value={to} onChange={(e) => setTo(e.target.value)}>
                {availableCurrencies.map(c => (
                  <option key={c} value={c}>{c}</option>
                ))}
              </select>
            </div>

            <div className="result-box">
              <label>Результат</label>
              <div className="result-display">
                {result ? result.result.toFixed(4) : "—"}
              </div>
            </div>
          </div>

          <button
            className="btn primary large"
            onClick={handleConvert}
            disabled={loading.convert}
          >
            {loading.convert ? "Конвертируем..." : "Конвертировать"}
          </button>

          {result && (
            <div className="success-box">
              <strong>
                {result.amount} {result.fromCurrency} → {result.result.toFixed(6)} {result.toCurrency}
              </strong>
              <br />
              <small>Курс: 1 {from} = {result.exchangeRate.toFixed(6)} {to}</small>
            </div>
          )}
        </section>

        {/* Управление данными */}
        <section className="card actions-card">
          <h2>Управление данными</h2>
          <div className="actions-grid">
            <button className="btn secondary" onClick={loadRates} disabled={loading.rates}>
              {loading.rates ? "Загрузка..." : "Обновить курсы"}
            </button>
            <button className="btn secondary" onClick={loadHistory} disabled={loading.history}>
              {loading.history ? "Загрузка..." : "Обновить историю"}
            </button>
          </div>
        </section>

        {/* Курсы */}
        {Object.keys(rates).length > 0 && (
          <section className="card rates-card">
            <h2>
              Актуальные курсы валют
              <span className="updated">
                Обновлено: {updatedAt ? format(new Date(updatedAt), "dd.MM.yyyy HH:mm:ss") : "—"}
              </span>
            </h2>
            <div className="rates-grid">
              {Object.entries(rates).map(([code, rate]) => (
                <div key={code} className="rate-item">
                  <div className="currency-code">{code}</div>
                  <div className="rate-value">{rate.toFixed(4)}</div>
                  <div className="rate-label">RUB</div>
                </div>
              ))}
            </div>
          </section>
        )}

        {/* История */}
        {history.length > 0 && (
          <section className="card history-card">
            <h2>Последние конвертации</h2>
            <div className="history-list">
              {history.slice(0, 15).map((conv) => (
                <div key={conv.id} className="history-item">
                  <div>
                    <strong>{conv.amount} {conv.fromCurrency}</strong> →{" "}
                    <strong className="text-indigo">{conv.result.toFixed(2)} {conv.toCurrency}</strong>
                  </div>
                  <div className="timestamp">
                    {format(new Date(conv.timestamp), "HH:mm:ss")}
                  </div>
                </div>
              ))}
            </div>
          </section>
        )}

        {error && <div className="error-box">{error}</div>}
      </div>
    </div>
  );
}

export default App;