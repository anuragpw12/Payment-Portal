# Payments Portal UI

Frontend application for managing payments, built with Angular 15.

## Overview

This app provides:
- Payments list view
- Create payment form
- Edit payment form
- Delete confirmation with SweetAlert2
- Success/error notifications with ngx-toastr

## Tech Stack

- Angular 15
- TypeScript
- Bootstrap 5
- Reactive Forms
- HttpClient
- ngx-toastr
- SweetAlert2

## Prerequisites

- Node.js 18+ (LTS recommended)
- npm

## Getting Started

1. Install dependencies:

```bash
npm install
```

2. Start the app:

```bash
npm start
```

3. Open in browser:

- `http://localhost:4200`

## Available Scripts

- `npm start` - Run development server
- `npm run build` - Production build
- `npm run watch` - Development build in watch mode
- `npm test` - Run unit tests

## Routing

- `/payments` - Payments list
- `/payments/new` - Create payment
- `/payments/:id/edit` - Edit payment
- Unknown routes redirect to `/payments`

## Environment Configuration

API URL is configured in:
- `src/environments/enviornement.ts`

Current value:
- `apiUrl: 'http://localhost:5106'`

The payments API base used by the app:
- `${apiUrl}/api/payments`

## Main Features

### Payments List

- Loads payments with pagination params (`pageNumber=1`, `pageSize=20`)
- Shows Reference, Amount, Currency, Created At
- Actions: Edit, Delete
- Delete uses SweetAlert2 Yes/No confirmation
- Shows toast on delete success/failure

### Payment Form

- Supports create and edit mode
- Fields: `amount`, `currency`
- Validation:
  - Amount required and must be `> 0`
  - Currency required
- Create flow:
  - Generates `clientRequestId` (UUID)
  - Calls `POST /api/payments`
- Edit flow:
  - Loads payment by route `id`
  - Calls `PUT /api/payments/{id}`
- Shows toastr for create/update success/failure

## API Service

`src/app/payments/services/payments-api.service.ts`

Methods:
- `getPayments(query)` -> `GET /api/payments`
- `createPayment(payload)` -> `POST /api/payments`
- `updatePayment(id, payload)` -> `PUT /api/payments/{id}`
- `deletePayment(id)` -> `DELETE /api/payments/{id}`

Note:
- `getPaymentById(id)` currently finds the payment by calling `getPayments({ pageNumber: 1, pageSize: 500 })` and filtering client-side.

## Project Structure

- `src/app/app.module.ts` - Root module and toastr setup
- `src/app/app-routing.module.ts` - App-level routes
- `src/app/payments/payments.module.ts` - Feature module
- `src/app/payments/payments-routing.module.ts` - Payments routes
- `src/app/payments/pages/payments-list/` - List page
- `src/app/payments/pages/payment-form/` - Create/Edit page
- `src/app/payments/services/payments-api.service.ts` - API integration
- `src/app/payments/models/payment.model.ts` - Data models

## Build Notes

- Build may show a CommonJS warning for `sweetalert2` in Angular 15.
- Bundle budget warning is currently expected with the existing config.
