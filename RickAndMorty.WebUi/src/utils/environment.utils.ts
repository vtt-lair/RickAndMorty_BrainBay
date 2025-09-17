export function getBaseUrl() {
    const url = import.meta.env.VITE_API_BASEURL;
    if (url === undefined) {
        throw new Error('VITE_API_BASEURL not set');
    }

    return url;
}