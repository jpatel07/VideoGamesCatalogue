export interface VideoGame {
  id: number;
  name: string;
  datePublished: string;
  author: string;
  description: string;
  gamePlatform: string;
  genre: string;
  aggregateRating: number;
}

export interface PageResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}
