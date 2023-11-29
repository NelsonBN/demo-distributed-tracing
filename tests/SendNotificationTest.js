import { check, sleep } from 'k6';
import http from 'k6/http';

const BASE_URL = 'http://localhost:8088/notifications';

const params = {
    headers: {
        'Content-Type': 'application/json',
    }
};

export default () => {
    const payload = JSON.stringify({
        "userId": "ab497ca3-4304-47fd-866d-b647f4daaa99",
        "message": "Hello World!"
    });

    let res = http.post(BASE_URL, payload, params);

    check(res, {
        'is status 202': (r) => r.status === 202,
    });

    sleep(1);
}