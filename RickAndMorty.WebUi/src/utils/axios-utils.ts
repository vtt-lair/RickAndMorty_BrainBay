import axios from 'axios';

type Data = {
    [key: string]: any
}

export async function get<T>(path: string): Promise<T> {
    const res = await axios.get(`${path}`, { maxRedirects: 0 })
        .then((response) => response.data);

    return res;
}

export async function post<T>(path: string, data: Data): Promise<T> {
    const res = await axios.post(`${path}`, data, { maxRedirects: 0 })
        .then((response) => response.data);

    return res;
}

export async function httpDelete<T>(path: string): Promise<T> {
    const res = await axios.delete(`${path}`, { maxRedirects: 0 })
        .then((response) => response.data);

    return res;
}