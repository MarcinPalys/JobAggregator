export interface JobOffer {
  id: string;
  title: string;
  company: string;
  location: string;
  city?: string;
  category?: string;
  description?: string;
  salaryMin?: number;
  salaryMax?: number;
  currency?: string;
  sourceName: string;
  sourceUrl: string;
  fetchedAt: string;
  publishedAt?: string;
}