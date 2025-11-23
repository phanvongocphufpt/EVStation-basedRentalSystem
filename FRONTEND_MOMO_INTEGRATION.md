# Hướng dẫn tích hợp MoMo Payment cho Frontend

## Tổng quan Flow

1. **Tạo Payment Request** → Nhận `momoPayUrl`
2. **Redirect user** đến `momoPayUrl` để thanh toán
3. **User thanh toán** trên MoMo
4. **MoMo redirect** user về `RedirectUrl` (frontend)
5. **Kiểm tra payment status** và hiển thị kết quả

---

## 1. Tạo Payment Request

### API Endpoint
```
POST /api/Payment/CreateMomoPayment?rentalOrderId={orderId}&userId={userId}&amount={amount}
```

### Request Headers
```
Authorization: Bearer {token}
Content-Type: application/json
```

### Response
```json
{
  "isSuccess": true,
  "data": {
    "momoPayUrl": "https://payment.momo.vn/...",
    "momoOrderId": "767d43c2-030e-4a2c-bb3a-0012e13fd0f9",
    "momoRequestId": "cb25ea0f-478d-4543-8dae-a3ba9fd06d2b",
    "status": "Pending"
  },
  "message": null
}
```

### Code mẫu (React/Next.js)

```typescript
// services/paymentService.ts
import axios from 'axios';

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'https://localhost:7200';

export interface CreateMomoPaymentResponse {
  momoPayUrl: string;
  momoOrderId: string;
  momoRequestId: string;
  status: string;
}

export const createMomoPayment = async (
  rentalOrderId: number,
  userId: number,
  amount: number,
  token: string
): Promise<CreateMomoPaymentResponse> => {
  const response = await axios.post<{
    isSuccess: boolean;
    data: CreateMomoPaymentResponse;
    message: string | null;
  }>(
    `${API_BASE_URL}/api/Payment/CreateMomoPayment`,
    null,
    {
      params: {
        rentalOrderId,
        userId,
        amount
      },
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    }
  );

  if (!response.data.isSuccess) {
    throw new Error(response.data.message || 'Tạo payment thất bại');
  }

  return response.data.data;
};
```

---

## 2. Redirect User đến MoMo

### Code mẫu (React Component)

```typescript
// components/PaymentButton.tsx
import { useState } from 'react';
import { createMomoPayment } from '@/services/paymentService';

interface PaymentButtonProps {
  rentalOrderId: number;
  userId: number;
  amount: number;
  token: string;
}

export const PaymentButton: React.FC<PaymentButtonProps> = ({
  rentalOrderId,
  userId,
  amount,
  token
}) => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handlePayment = async () => {
    try {
      setLoading(true);
      setError(null);

      // Tạo payment request
      const paymentData = await createMomoPayment(
        rentalOrderId,
        userId,
        amount,
        token
      );

      // Redirect user đến MoMo
      if (paymentData.momoPayUrl) {
        window.location.href = paymentData.momoPayUrl;
      } else {
        throw new Error('Không nhận được payment URL từ MoMo');
      }
    } catch (err: any) {
      setError(err.message || 'Có lỗi xảy ra khi tạo payment');
      setLoading(false);
    }
  };

  return (
    <div>
      <button
        onClick={handlePayment}
        disabled={loading}
        className="bg-blue-500 text-white px-6 py-2 rounded disabled:opacity-50"
      >
        {loading ? 'Đang xử lý...' : 'Thanh toán bằng MoMo'}
      </button>
      {error && (
        <div className="mt-2 text-red-500 text-sm">{error}</div>
      )}
    </div>
  );
};
```

---

## 3. Xử lý Callback từ MoMo

### MoMo sẽ redirect user về `RedirectUrl` với các query params:
```
https://localhost:3000/payment-success?orderId=xxx&resultCode=0&message=Success
```

### Code mẫu (Payment Success Page)

```typescript
// pages/payment-success.tsx hoặc app/payment-success/page.tsx
import { useEffect, useState } from 'react';
import { useRouter, useSearchParams } from 'next/navigation';
import { getPaymentByMomoOrderId } from '@/services/paymentService';

export default function PaymentSuccessPage() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const [paymentStatus, setPaymentStatus] = useState<'loading' | 'success' | 'failed'>('loading');
  const [message, setMessage] = useState<string>('');

  useEffect(() => {
    const checkPaymentStatus = async () => {
      const orderId = searchParams.get('orderId');
      const resultCode = searchParams.get('resultCode');

      if (!orderId) {
        setPaymentStatus('failed');
        setMessage('Không tìm thấy thông tin đơn hàng');
        return;
      }

      try {
        // Kiểm tra payment status từ backend
        const payment = await getPaymentByMomoOrderId(orderId, token);
        
        if (payment.status === 'Completed' || resultCode === '0') {
          setPaymentStatus('success');
          setMessage('Thanh toán thành công!');
          
          // Redirect đến trang đơn hàng sau 3 giây
          setTimeout(() => {
            router.push(`/orders/${payment.rentalOrderId}`);
          }, 3000);
        } else {
          setPaymentStatus('failed');
          setMessage('Thanh toán thất bại. Vui lòng thử lại.');
        }
      } catch (error) {
        setPaymentStatus('failed');
        setMessage('Có lỗi xảy ra khi kiểm tra trạng thái thanh toán');
      }
    };

    checkPaymentStatus();
  }, [searchParams, router]);

  return (
    <div className="min-h-screen flex items-center justify-center">
      <div className="text-center">
        {paymentStatus === 'loading' && (
          <>
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-500 mx-auto"></div>
            <p className="mt-4">Đang kiểm tra thanh toán...</p>
          </>
        )}
        
        {paymentStatus === 'success' && (
          <>
            <div className="text-green-500 text-6xl mb-4">✓</div>
            <h1 className="text-2xl font-bold text-green-500">Thanh toán thành công!</h1>
            <p className="mt-2">{message}</p>
            <p className="mt-4 text-gray-500">Đang chuyển hướng...</p>
          </>
        )}
        
        {paymentStatus === 'failed' && (
          <>
            <div className="text-red-500 text-6xl mb-4">✗</div>
            <h1 className="text-2xl font-bold text-red-500">Thanh toán thất bại</h1>
            <p className="mt-2">{message}</p>
            <button
              onClick={() => router.push('/checkout')}
              className="mt-4 bg-blue-500 text-white px-6 py-2 rounded"
            >
              Thử lại
            </button>
          </>
        )}
      </div>
    </div>
  );
}
```

---

## 4. API kiểm tra Payment Status

### Endpoint
```
GET /api/Payment/GetByMomoOrderId?momoOrderId={orderId}
```

### Response
```json
{
  "isSuccess": true,
  "data": {
    "paymentId": 123,
    "paymentType": "OrderPayment",
    "paymentDate": "2024-11-23T10:00:00Z",
    "amount": 500000,
    "paymentMethod": "MoMo",
    "status": "Completed",
    "rentalOrderId": 456,
    "user": {
      "userId": 1,
      "email": "user@example.com",
      "fullName": "Nguyễn Văn A",
      "role": "Customer"
    },
    "order": {
      "orderId": 456,
      "orderDate": "2024-11-23T09:00:00Z",
      "total": 500000
    },
    "momoOrderId": "767d43c2-030e-4a2c-bb3a-0012e13fd0f9",
    "momoRequestId": "cb25ea0f-478d-4543-8dae-a3ba9fd06d2b",
    "momoResultCode": 0
  }
}
```

### Code mẫu

```typescript
// services/paymentService.ts
export interface PaymentDetail {
  paymentId: number;
  paymentType: string;
  paymentDate: string | null;
  amount: number;
  paymentMethod: string | null;
  status: string;
  rentalOrderId: number | null;
  user: {
    userId: number;
    email: string;
    fullName: string;
    role: string;
  } | null;
  order: {
    orderId: number;
    orderDate: string;
    total: number | null;
  } | null;
  momoOrderId: string | null;
  momoRequestId: string | null;
  momoResultCode: number | null;
}

export const getPaymentByMomoOrderId = async (
  momoOrderId: string,
  token: string
): Promise<PaymentDetail> => {
  const response = await axios.get<{
    isSuccess: boolean;
    data: PaymentDetail;
    message: string | null;
  }>(
    `${API_BASE_URL}/api/Payment/GetByMomoOrderId`,
    {
      params: { momoOrderId },
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    }
  );

  if (!response.data.isSuccess) {
    throw new Error(response.data.message || 'Không tìm thấy payment');
  }

  return response.data.data;
};
```

---

## 5. Flow hoàn chỉnh (Vue.js example)

```vue
<!-- components/MomoPayment.vue -->
<template>
  <div>
    <button 
      @click="initiatePayment" 
      :disabled="loading"
      class="payment-button"
    >
      {{ loading ? 'Đang xử lý...' : 'Thanh toán MoMo' }}
    </button>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { createMomoPayment } from '@/services/paymentService';

const props = defineProps<{
  orderId: number;
  userId: number;
  amount: number;
}>();

const router = useRouter();
const loading = ref(false);

const initiatePayment = async () => {
  try {
    loading.value = true;
    
    const token = localStorage.getItem('token');
    if (!token) {
      alert('Vui lòng đăng nhập');
      return;
    }

    const paymentData = await createMomoPayment(
      props.orderId,
      props.userId,
      props.amount,
      token
    );

    // Redirect đến MoMo
    if (paymentData.momoPayUrl) {
      window.location.href = paymentData.momoPayUrl;
    } else {
      throw new Error('Không nhận được payment URL');
    }
  } catch (error: any) {
    alert(error.message || 'Có lỗi xảy ra');
    loading.value = false;
  }
};
</script>
```

---

## 6. Checklist cho Frontend

- [ ] Tạo service function để gọi API `CreateMomoPayment`
- [ ] Tạo component/button để khởi tạo payment
- [ ] Xử lý redirect đến `momoPayUrl`
- [ ] Tạo page xử lý callback từ MoMo (`RedirectUrl`)
- [ ] Tạo service function để check payment status
- [ ] Hiển thị UI cho các trạng thái: loading, success, failed
- [ ] Xử lý error cases
- [ ] Test với MoMo sandbox/test environment

---

## 7. Lưu ý quan trọng

1. **Environment Variables**: Đảm bảo `RedirectUrl` trong backend config khớp với frontend URL
2. **HTTPS**: MoMo yêu cầu HTTPS cho production (có thể dùng ngrok cho local testing)
3. **Error Handling**: Luôn xử lý các trường hợp lỗi và hiển thị thông báo rõ ràng
4. **Security**: Không lưu sensitive data (token, payment info) trong localStorage/sessionStorage
5. **Testing**: Test kỹ với MoMo sandbox trước khi deploy production

---

## 8. Example với Axios Interceptor

```typescript
// utils/axios.ts
import axios from 'axios';

const apiClient = axios.create({
  baseURL: process.env.NEXT_PUBLIC_API_URL,
});

// Add token to all requests
apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Handle errors
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      // Redirect to login
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export default apiClient;
```

