import type { Expense, ExpenseFilters, TotalExpenseParams } from "@/types"

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || "https://localhost:5000"
const API_VERSION = "api/v1/expense"

export class ExpenseAPI {
  private static getUrl(endpoint: string) {
    return `${API_BASE_URL}/${API_VERSION}${endpoint}`
  }

  static async createExpense(expense: Omit<Expense, "id" | "dateRecorded">) {
    const response = await fetch(this.getUrl(""), {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(expense),
    })
    return response.json()
  }

  static async getAllExpenses(): Promise<Expense[]> {
    const response = await fetch(this.getUrl("/getall"))
    return response.json()
  }

  static async getExpenseById(id: string): Promise<Expense> {
    const response = await fetch(this.getUrl(`/${id}`))
    return response.json()
  }

  static async getFilteredExpenses(filters: ExpenseFilters): Promise<any> {
    const params = new URLSearchParams()
    Object.entries(filters).forEach(([key, value]) => {
      if (value !== undefined && value !== null && value !== "") {
        params.append(key, value.toString())
      }
    })

    const response = await fetch(this.getUrl(`/filter?${params}`))
    return response.json()
  }

  static async getTotalExpense(params: TotalExpenseParams): Promise<any> {
    const searchParams = new URLSearchParams()
    Object.entries(params).forEach(([key, value]) => {
      if (value !== undefined && value !== null && value !== "") {
        searchParams.append(key, value.toString())
      }
    })

    const response = await fetch(this.getUrl(`/total?${searchParams}`))
    return response.json()
  }

  static async updateExpense(id: string, expense: Expense) {
    const response = await fetch(this.getUrl(`/${id}`), {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(expense),
    })
    return response.json()
  }

  static async deleteExpense(id: string) {
    const response = await fetch(this.getUrl(`/${id}`), {
      method: "DELETE",
    })
    return response.ok
  }

  static async deleteAllExpenses() {
    const response = await fetch(this.getUrl("/all"), {
      method: "DELETE",
    })
    return response.ok
  }
}
