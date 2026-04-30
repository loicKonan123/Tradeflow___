import { getFirebaseAuth } from "./firebase";

const BASE = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5175";

async function publicGet<T>(path: string): Promise<T> {
  const res = await fetch(`${BASE}${path}`);
  if (res.status === 404) throw new Error("NOT_FOUND");
  if (!res.ok) throw new Error(`GET ${path} → ${res.status}`);
  return res.json();
}

async function authHeaders(): Promise<HeadersInit> {
  const user = getFirebaseAuth().currentUser;
  if (!user) return { "Content-Type": "application/json" };
  const token = await user.getIdToken();
  return {
    "Content-Type": "application/json",
    Authorization: `Bearer ${token}`,
  };
}

async function get<T>(path: string): Promise<T> {
  const res = await fetch(`${BASE}${path}`, {
    headers: await authHeaders(),
  });
  if (!res.ok) throw new Error(`GET ${path} → ${res.status}`);
  return res.json();
}

async function post<T>(path: string, body?: unknown): Promise<T> {
  const res = await fetch(`${BASE}${path}`, {
    method: "POST",
    headers: await authHeaders(),
    body: body !== undefined ? JSON.stringify(body) : undefined,
  });
  if (!res.ok) {
    const err = await res.json().catch(() => ({}));
    throw new Error((err as { error?: string }).error ?? `POST ${path} → ${res.status}`);
  }
  return res.json();
}

// --- Public catalog ---
export interface ProductMedia {
  id: string;
  url: string;
  type: string;
  sortOrder: number;
}

export interface BacktestReport {
  id: string;
  title: string;
  winRate: number | null;
  maxDrawdown: number | null;
  profitFactor: number | null;
  periodStart: string | null;
  periodEnd: string | null;
  markets: string | null;
}

export interface ProductDetail {
  id: string;
  title: string;
  slug: string;
  shortDescription: string;
  longDescriptionMarkdown: string | null;
  type: string;
  price: number;
  currency: string;
  status: string;
  salesCount: number;
  medias: ProductMedia[];
  backtestReports: BacktestReport[];
  publishedAt: string | null;
}

export const getProduct = (slug: string) =>
  publicGet<ProductDetail>(`/api/catalog/${slug}`);

// --- Account ---
export const syncAccount = () => post<{ customerId: string }>("/api/account/sync");

// --- Licenses ---
export interface License {
  id: string;
  productId: string;
  productName: string;
  productSlug: string;
  downloadToken: string;
  downloadsUsed: number;
  maxDownloads: number;
  isRevoked: boolean;
  createdAt: string;
}

export const getLicenses = () => get<License[]>("/api/licenses");
export const downloadLicense = (id: string) =>
  post<{ url: string }>(`/api/licenses/${id}/download`);

// --- Orders ---
export interface OrderItem {
  productId: string;
  productName: string;
  unitPrice: number;
  currency: string;
}

export interface Order {
  id: string;
  orderNumber: string;
  status: string;
  totalAmount: number;
  currency: string;
  items: OrderItem[];
  createdAt: string;
  paidAt: string | null;
}

export const getOrders = () => get<Order[]>("/api/orders");
export const createOrder = (productIds: string[], currency = "EUR") =>
  post<{ checkoutUrl: string }>("/api/orders", { productIds, currency });

// --- Projects ---
export type ProjectStatus =
  | "Submitted"
  | "Quoted"
  | "DepositPaid"
  | "InProgress"
  | "Delivered"
  | "Cancelled";

export interface Project {
  id: string;
  strategyTitle: string;
  market: string;
  timeframe: string;
  status: ProjectStatus;
  entryConditions: string;
  exitConditions: string;
  riskManagement: string;
  indicators: string;
  additionalNotes: string | null;
  strategyType: string | null;
  tradingViewChartUrl: string | null;
  budgetRange: string | null;
  desiredDeadline: string | null;
  attachmentUrl: string | null;
  attachmentName: string | null;
  quotedPrice: number | null;
  quotedCurrency: string | null;
  adminNotes: string | null;
  depositAmount: number | null;
  deliveryNotes: string | null;
  createdAt: string;
  deliveredAt: string | null;
}

export interface SubmitProjectPayload {
  strategyTitle: string;
  market: string;
  timeframe: string;
  entryConditions: string;
  exitConditions: string;
  riskManagement: string;
  indicators: string;
  additionalNotes?: string;
  strategyType?: string;
  tradingViewChartUrl?: string;
  budgetRange?: string;
  desiredDeadline?: string;
}

export const getProjects = () => get<Project[]>("/api/projects");
export const submitProject = (payload: SubmitProjectPayload) =>
  post<{ projectId: string }>("/api/projects", payload);
export const cancelProject = (id: string) =>
  post<void>(`/api/projects/${id}/cancel`);

export async function uploadProjectAttachment(id: string, file: File): Promise<void> {
  const user = getFirebaseAuth().currentUser;
  const token = user ? await user.getIdToken() : null;
  const form = new FormData();
  form.append("file", file);
  const res = await fetch(`${BASE}/api/projects/${id}/attachment`, {
    method: "POST",
    headers: token ? { Authorization: `Bearer ${token}` } : {},
    body: form,
  });
  if (!res.ok) {
    const err = await res.json().catch(() => ({}));
    throw new Error((err as { error?: string }).error ?? `Upload failed ${res.status}`);
  }
}

// --- Admin: Products ---
export interface AdminProduct {
  id: string;
  title: string;
  slug: string;
  type: string;
  price: number;
  currency: string;
  status: string;
  salesCount: number;
  fileUrl: string | null;
  createdAt: string;
}

export interface PagedResult<T> {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export const getAdminProducts = (page = 1, status?: string) =>
  get<PagedResult<AdminProduct>>(
    `/api/admin/products?page=${page}&pageSize=20${status ? `&status=${status}` : ""}`
  );

export const createProduct = (body: {
  title: string;
  shortDescription: string;
  longDescriptionMarkdown: string;
  type: string;
  price: number;
  currency: string;
  categoryId?: string;
}) => post<{ id: string }>("/api/admin/products", body);

export const publishProduct = (id: string) =>
  post<void>(`/api/admin/products/${id}/publish`);

export const archiveProduct = (id: string) =>
  post<void>(`/api/admin/products/${id}/archive`);

export async function uploadProductFile(id: string, file: File): Promise<{ url: string }> {
  const user = getFirebaseAuth().currentUser;
  const token = user ? await user.getIdToken() : null;
  const form = new FormData();
  form.append("file", file);
  const res = await fetch(
    `${BASE}/api/admin/products/${id}/file`,
    {
      method: "POST",
      headers: token ? { Authorization: `Bearer ${token}` } : {},
      body: form,
    }
  );
  if (!res.ok) {
    const err = await res.json().catch(() => ({}));
    throw new Error((err as { error?: string }).error ?? `Upload failed ${res.status}`);
  }
  return res.json();
}

// --- Admin: Orders ---
export interface AdminOrder {
  id: string;
  orderNumber: string;
  customerEmail: string;
  status: string;
  totalAmount: number;
  currency: string;
  itemCount: number;
  createdAt: string;
  paidAt: string | null;
}

export const getAdminOrders = (page = 1, status?: string) =>
  get<PagedResult<AdminOrder>>(
    `/api/admin/orders?page=${page}&pageSize=20${status ? `&status=${status}` : ""}`
  );

// --- Admin: Projects ---
export interface AdminProject extends Project {
  customerEmail: string;
  projectNumber: string;
}

export const getAdminProjects = (page = 1, status?: string) =>
  get<PagedResult<AdminProject>>(
    `/api/admin/projects?page=${page}&pageSize=20${status ? `&status=${status}` : ""}`
  );

export const sendQuote = (id: string, price: number, currency: string, adminNotes?: string) =>
  post<void>(`/api/admin/projects/${id}/quote`, { price, currency, adminNotes });

export const startProjectWork = (id: string) =>
  post<void>(`/api/admin/projects/${id}/start`);

export async function deliverProject(id: string, file: File, deliveryNotes?: string): Promise<void> {
  const user = getFirebaseAuth().currentUser;
  const token = user ? await user.getIdToken() : null;
  const form = new FormData();
  form.append("file", file);
  if (deliveryNotes) form.append("deliveryNotes", deliveryNotes);
  const res = await fetch(
    `${BASE}/api/admin/projects/${id}/deliver`,
    {
      method: "POST",
      headers: token ? { Authorization: `Bearer ${token}` } : {},
      body: form,
    }
  );
  if (!res.ok) {
    const err = await res.json().catch(() => ({}));
    throw new Error((err as { error?: string }).error ?? `Delivery failed ${res.status}`);
  }
}

export const revokeLicense = (id: string) =>
  post<void>(`/api/admin/licenses/${id}/revoke`);
