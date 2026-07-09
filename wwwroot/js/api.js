const API = {
    getToken: () => localStorage.getItem('token'),
    getUser: () => JSON.parse(localStorage.getItem('user') || 'null'),
    isAdmin: () => API.getUser()?.role === 'Admin',

    requireAuth() {
        if (!API.getToken()) { location.href = '/index.html'; return false; }
        return true;
    },

    logout() {
        localStorage.removeItem('token');
        localStorage.removeItem('user');
        location.href = '/index.html';
    },

    async fetch(url, options = {}) {
        const token = API.getToken();
        const res = await fetch(url, {
            ...options,
            headers: {
                'Content-Type': 'application/json',
                ...(token ? { 'Authorization': `Bearer ${token}` } : {}),
                ...options.headers
            }
        });
        if (res.status === 401) { API.logout(); return null; }
        return res;
    },

    async get(url) {
        const res = await API.fetch(url);
        if (!res) return null;
        return res.ok ? res.json() : null;
    },

    async post(url, body) {
        const res = await API.fetch(url, { method: 'POST', body: JSON.stringify(body) });
        if (!res) return { ok: false, data: null };
        const data = await res.json().catch(() => ({}));
        return { ok: res.ok, data };
    },

    async put(url, body) {
        const res = await API.fetch(url, { method: 'PUT', body: body !== undefined ? JSON.stringify(body) : undefined });
        if (!res) return { ok: false, data: null };
        const data = await res.json().catch(() => ({}));
        return { ok: res.ok, data };
    },

    async del(url) {
        const res = await API.fetch(url, { method: 'DELETE' });
        if (!res) return false;
        return res.ok;
    },
};

// Status/priority display helpers
const statusLabel = { Pending: 'Pending', InProgress: 'In Progress', Completed: 'Completed', Cancelled: 'Cancelled' };
const statusClass = { Pending: 'warning', InProgress: 'primary', Completed: 'success', Cancelled: 'secondary' };
const priorityLabel = { Low: 'Low', Medium: 'Medium', High: 'High' };
const priorityClass = { Low: 'success', Medium: 'warning', High: 'danger' };
const equipStatusLabel = { Normal: 'Normal', UnderRepair: 'Under Repair', Scrapped: 'Scrapped' };
const equipStatusClass = { Normal: 'success', UnderRepair: 'warning', Scrapped: 'secondary' };

function fmtDate(d) {
    if (!d) return '-';
    return new Date(d).toLocaleString('en-CA', { year: 'numeric', month: '2-digit', day: '2-digit', hour: '2-digit', minute: '2-digit' });
}
