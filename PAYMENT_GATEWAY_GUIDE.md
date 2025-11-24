# Hướng dẫn sử dụng Payment Gateway Selection

## 📋 Tổng quan

API này cho phép chọn payment gateway (MoMo, PayOS, Cash, BankTransfer) và tạo payment tương ứng.

## 🔗 API Endpoint

### Tạo Payment với Gateway được chọn

**Endpoint:** `POST /api/Payment/CreatePayment`

**Headers:**
```
Authorization: Bearer {token}
Content-Type: application/json
```

**Request Body:**
```json
{
  "rentalOrderId": 48,
  "userId": 1,
  "amount": 600000,
  "gateway": 2
}
```

**Gateway Values:**
- `0` = Cash
- `1` = BankTransfer
- `2` = MoMo
- `3` = PayOS

**Response Success (200):**
```json
{
  "success": true,
  "data": {
    "gateway": 2,
    "status": "Pending",
    "momoPayUrl": "https://payment.momo.vn/...",
    "momoOrderId": "xxx",
    "momoRequestId": "xxx"
  }
}
```

**Hoặc với PayOS:**
```json
{
  "success": true,
  "data": {
    "gateway": 3,
    "status": "Pending",
    "payOSCheckoutUrl": "https://pay.payos.vn/web/...",
    "payOSQrCode": "data:image/png;base64,...",
    "payOSOrderCode": 137900
  }
}
```

**Hoặc với Cash/BankTransfer:**
```json
{
  "success": true,
  "data": {
    "gateway": 0,
    "status": "Pending"
  }
}
```

**Response Error (400):**
```json
{
  "success": false,
  "message": "Số tiền tối thiểu là 1,000 VND"
}
```

## 💻 Code Examples

### React/TypeScript Example

```typescript
// types.ts
enum PaymentGateway {
  Cash = 0,
  BankTransfer = 1,
  MoMo = 2,
  PayOS = 3
}

interface CreatePaymentRequest {
  rentalOrderId: number;
  userId: number;
  amount: number;
  gateway: PaymentGateway;
}

interface CreatePaymentResponse {
  success: boolean;
  data?: {
    gateway: PaymentGateway;
    status: string;
    // MoMo fields
    momoPayUrl?: string;
    momoOrderId?: string;
    momoRequestId?: string;
    // PayOS fields
    payOSCheckoutUrl?: string;
    payOSQrCode?: string;
    payOSOrderCode?: number;
  };
  message?: string;
}

// PaymentService.ts
import axios from 'axios';

const API_BASE_URL = 'https://localhost:7200/api';

export class PaymentService {
  private static getAuthHeaders() {
    const token = localStorage.getItem('token');
    return {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    };
  }

  /**
   * Tạo payment với gateway được chọn
   */
  static async createPayment(
    rentalOrderId: number,
    userId: number,
    amount: number,
    gateway: PaymentGateway
  ): Promise<CreatePaymentResponse> {
    try {
      const response = await axios.post<CreatePaymentResponse>(
        `${API_BASE_URL}/Payment/CreatePayment`,
        {
          rentalOrderId,
          userId,
          amount,
          gateway
        },
        {
          headers: this.getAuthHeaders()
        }
      );
      return response.data;
    } catch (error: any) {
      if (error.response?.data) {
        return error.response.data;
      }
      throw error;
    }
  }
}
```

### React Component Example

```tsx
import React, { useState } from 'react';
import { PaymentService, PaymentGateway } from './services/PaymentService';

interface PaymentFormProps {
  rentalOrderId: number;
  userId: number;
  amount: number;
  onSuccess?: () => void;
  onError?: (message: string) => void;
}

export const PaymentForm: React.FC<PaymentFormProps> = ({
  rentalOrderId,
  userId,
  amount,
  onSuccess,
  onError
}) => {
  const [selectedGateway, setSelectedGateway] = useState<PaymentGateway>(PaymentGateway.MoMo);
  const [loading, setLoading] = useState(false);
  const [paymentData, setPaymentData] = useState<any>(null);

  const handlePayment = async () => {
    setLoading(true);
    try {
      const response = await PaymentService.createPayment(
        rentalOrderId,
        userId,
        amount,
        selectedGateway
      );

      if (response.success && response.data) {
        setPaymentData(response.data);
        
        // Xử lý theo gateway
        if (selectedGateway === PaymentGateway.MoMo && response.data.momoPayUrl) {
          // Redirect đến MoMo
          window.location.href = response.data.momoPayUrl;
        } else if (selectedGateway === PaymentGateway.PayOS && response.data.payOSCheckoutUrl) {
          // Redirect đến PayOS hoặc hiển thị QR Code
          if (response.data.payOSQrCode) {
            // Hiển thị QR Code modal
            showQRCodeModal(response.data.payOSQrCode, response.data.payOSCheckoutUrl);
          } else {
            window.location.href = response.data.payOSCheckoutUrl;
          }
        } else if (selectedGateway === PaymentGateway.Cash || selectedGateway === PaymentGateway.BankTransfer) {
          // Cash/BankTransfer - chỉ cần hiển thị thông báo
          alert('Payment đã được tạo. Vui lòng thanh toán trực tiếp.');
          onSuccess?.();
        }
      } else {
        onError?.(response.message || 'Tạo payment thất bại');
      }
    } catch (error: any) {
      onError?.(error.message || 'Có lỗi xảy ra');
    } finally {
      setLoading(false);
    }
  };

  const showQRCodeModal = (qrCode: string, checkoutUrl: string) => {
    // Implement QR Code modal
    // Có thể dùng library như react-qr-code hoặc hiển thị image
  };

  return (
    <div className="payment-form">
      <h3>Chọn phương thức thanh toán</h3>
      
      <div className="gateway-selection">
        <label>
          <input
            type="radio"
            value={PaymentGateway.MoMo}
            checked={selectedGateway === PaymentGateway.MoMo}
            onChange={(e) => setSelectedGateway(Number(e.target.value))}
          />
          MoMo
        </label>
        
        <label>
          <input
            type="radio"
            value={PaymentGateway.PayOS}
            checked={selectedGateway === PaymentGateway.PayOS}
            onChange={(e) => setSelectedGateway(Number(e.target.value))}
          />
          PayOS
        </label>
        
        <label>
          <input
            type="radio"
            value={PaymentGateway.Cash}
            checked={selectedGateway === PaymentGateway.Cash}
            onChange={(e) => setSelectedGateway(Number(e.target.value))}
          />
          Tiền mặt
        </label>
        
        <label>
          <input
            type="radio"
            value={PaymentGateway.BankTransfer}
            checked={selectedGateway === PaymentGateway.BankTransfer}
            onChange={(e) => setSelectedGateway(Number(e.target.value))}
          />
          Chuyển khoản
        </label>
      </div>

      <div className="amount-info">
        <p>Số tiền: {amount.toLocaleString('vi-VN')} VND</p>
      </div>

      <button
        onClick={handlePayment}
        disabled={loading}
        className="btn btn-primary"
      >
        {loading ? 'Đang xử lý...' : 'Thanh toán'}
      </button>

      {paymentData?.payOSQrCode && (
        <div className="qr-code-modal">
          <h4>Quét mã QR để thanh toán</h4>
          <img src={paymentData.payOSQrCode} alt="QR Code" />
          <a href={paymentData.payOSCheckoutUrl} target="_blank">
            Hoặc mở trang thanh toán
          </a>
        </div>
      )}
    </div>
  );
};
```

### Vue 3 Example

```vue
<template>
  <div class="payment-form">
    <h3>Chọn phương thức thanh toán</h3>
    
    <div class="gateway-selection">
      <label v-for="gateway in gateways" :key="gateway.value">
        <input
          type="radio"
          :value="gateway.value"
          v-model="selectedGateway"
        />
        {{ gateway.label }}
      </label>
    </div>

    <div class="amount-info">
      <p>Số tiền: {{ formatAmount(amount) }} VND</p>
    </div>

    <button @click="handlePayment" :disabled="loading">
      {{ loading ? 'Đang xử lý...' : 'Thanh toán' }}
    </button>

    <div v-if="paymentData?.payOSQrCode" class="qr-code-modal">
      <h4>Quét mã QR để thanh toán</h4>
      <img :src="paymentData.payOSQrCode" alt="QR Code" />
      <a :href="paymentData.payOSCheckoutUrl" target="_blank">
        Hoặc mở trang thanh toán
      </a>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import axios from 'axios';

const props = defineProps<{
  rentalOrderId: number;
  userId: number;
  amount: number;
}>();

const emit = defineEmits<{
  success: [];
  error: [message: string];
}>();

enum PaymentGateway {
  Cash = 0,
  BankTransfer = 1,
  MoMo = 2,
  PayOS = 3
}

const gateways = [
  { value: PaymentGateway.MoMo, label: 'MoMo' },
  { value: PaymentGateway.PayOS, label: 'PayOS' },
  { value: PaymentGateway.Cash, label: 'Tiền mặt' },
  { value: PaymentGateway.BankTransfer, label: 'Chuyển khoản' }
];

const selectedGateway = ref<PaymentGateway>(PaymentGateway.MoMo);
const loading = ref(false);
const paymentData = ref<any>(null);

const API_BASE_URL = 'https://localhost:7200/api';

const formatAmount = (amount: number) => {
  return amount.toLocaleString('vi-VN');
};

const handlePayment = async () => {
  loading.value = true;
  try {
    const token = localStorage.getItem('token');
    const response = await axios.post(
      `${API_BASE_URL}/Payment/CreatePayment`,
      {
        rentalOrderId: props.rentalOrderId,
        userId: props.userId,
        amount: props.amount,
        gateway: selectedGateway.value
      },
      {
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      }
    );

    if (response.data.success && response.data.data) {
      paymentData.value = response.data.data;
      
      // Xử lý theo gateway
      if (selectedGateway.value === PaymentGateway.MoMo && response.data.data.momoPayUrl) {
        window.location.href = response.data.data.momoPayUrl;
      } else if (selectedGateway.value === PaymentGateway.PayOS && response.data.data.payOSCheckoutUrl) {
        if (response.data.data.payOSQrCode) {
          // Hiển thị QR Code
        } else {
          window.location.href = response.data.data.payOSCheckoutUrl;
        }
      } else {
        alert('Payment đã được tạo. Vui lòng thanh toán trực tiếp.');
        emit('success');
      }
    } else {
      emit('error', response.data.message || 'Tạo payment thất bại');
    }
  } catch (error: any) {
    emit('error', error.response?.data?.message || 'Có lỗi xảy ra');
  } finally {
    loading.value = false;
  }
};
</script>
```

## 📊 Response Fields theo Gateway

### MoMo (gateway = 2)
```typescript
{
  gateway: 2,
  status: "Pending",
  momoPayUrl: string,      // URL để redirect
  momoOrderId: string,     // MoMo order ID
  momoRequestId: string    // MoMo request ID
}
```

### PayOS (gateway = 3)
```typescript
{
  gateway: 3,
  status: "Pending",
  payOSCheckoutUrl: string,  // URL để redirect
  payOSQrCode: string,       // QR Code base64
  payOSOrderCode: number     // PayOS order code
}
```

### Cash/BankTransfer (gateway = 0 hoặc 1)
```typescript
{
  gateway: 0 | 1,
  status: "Pending"
  // Không có thêm field nào
}
```

## 🔄 Flow xử lý

```
1. User chọn payment gateway (MoMo/PayOS/Cash/BankTransfer)
   ↓
2. Frontend gọi API: POST /api/Payment/CreatePayment
   ↓
3. Backend route đến gateway tương ứng
   ↓
4. Backend trả về response với fields tương ứng
   ↓
5. Frontend xử lý response:
   - MoMo: Redirect đến momoPayUrl
   - PayOS: Hiển thị QR Code hoặc redirect đến payOSCheckoutUrl
   - Cash/BankTransfer: Hiển thị thông báo thanh toán trực tiếp
```

## ✅ Validation

- **Amount:** Phải từ 1,000 đến 50,000,000 VND
- **Gateway:** Phải là một trong các giá trị: 0, 1, 2, 3
- **RentalOrderId:** Phải tồn tại trong hệ thống
- **UserId:** Phải tồn tại trong hệ thống

## 🎯 Best Practices

1. **Luôn kiểm tra `success` field** trước khi xử lý data
2. **Xử lý error** một cách graceful
3. **Hiển thị loading state** khi đang tạo payment
4. **Redirect user** đến gateway payment page khi có URL
5. **Lưu payment info** để có thể kiểm tra lại sau

## 📝 Notes

- Cash và BankTransfer không cần redirect, chỉ cần tạo payment record
- MoMo và PayOS sẽ redirect user đến trang thanh toán
- Backend tự động xử lý IPN callback từ MoMo/PayOS
- Frontend chỉ cần xử lý returnUrl callback

