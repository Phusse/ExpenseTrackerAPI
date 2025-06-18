export interface Expense {
  id: string
  category: string
  amount: number
  dateOfExpense: string
  description: string
  dateRecorded: string
}

export interface ExpenseFilters {
  startDate?: string
  endDate?: string
  minAmount?: number
  maxAmount?: number
  exactAmount?: number
  category?: string
}

export interface TotalExpenseParams {
  startDate?: string
  endDate?: string
  month?: number
  year?: number
}
