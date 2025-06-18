"use client"

import type React from "react"

import { useState } from "react"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Textarea } from "@/components/ui/textarea"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select"
import { ExpenseAPI } from "@/lib/expense-api"
import type { Expense } from "@/types/expense"
import { toast } from "@/hooks/use-toast"

interface ExpenseFormProps {
  expense?: Expense
  onSuccess?: () => void
  onCancel?: () => void
}

const categories = [
  "Food & Dining",
  "Transportation",
  "Shopping",
  "Entertainment",
  "Bills & Utilities",
  "Healthcare",
  "Travel",
  "Education",
  "Business",
  "Other",
]

export function ExpenseForm({ expense, onSuccess, onCancel }: ExpenseFormProps) {
  const [formData, setFormData] = useState({
    category: expense?.category || "",
    amount: expense?.amount?.toString() || "",
    dateOfExpense: expense?.dateOfExpense?.split("T")[0] || new Date().toISOString().split("T")[0],
    description: expense?.description || "",
  })
  const [isLoading, setIsLoading] = useState(false)

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setIsLoading(true)

    try {
      const expenseData = {
        ...formData,
        amount: Number.parseFloat(formData.amount),
        dateOfExpense: new Date(formData.dateOfExpense).toISOString(),
      }

      if (expense) {
        await ExpenseAPI.updateExpense(expense.id, { ...expense, ...expenseData })
        toast({ title: "Success", description: "Expense updated successfully!" })
      } else {
        await ExpenseAPI.createExpense(expenseData)
        toast({ title: "Success", description: "Expense created successfully!" })
      }

      onSuccess?.()
    } catch (error) {
      toast({
        title: "Error",
        description: "Failed to save expense. Please try again.",
        variant: "destructive",
      })
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <Card className="w-full max-w-md">
      <CardHeader>
        <CardTitle>{expense ? "Edit Expense" : "Add New Expense"}</CardTitle>
      </CardHeader>
      <CardContent>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <Label htmlFor="category">Category</Label>
            <Select
              value={formData.category}
              onValueChange={(value) => setFormData((prev) => ({ ...prev, category: value }))}
            >
              <SelectTrigger>
                <SelectValue placeholder="Select a category" />
              </SelectTrigger>
              <SelectContent>
                {categories.map((category) => (
                  <SelectItem key={category} value={category}>
                    {category}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>

          <div>
            <Label htmlFor="amount">Amount</Label>
            <Input
              id="amount"
              type="number"
              step="0.01"
              placeholder="0.00"
              value={formData.amount}
              onChange={(e) => setFormData((prev) => ({ ...prev, amount: e.target.value }))}
              required
            />
          </div>

          <div>
            <Label htmlFor="dateOfExpense">Date</Label>
            <Input
              id="dateOfExpense"
              type="date"
              value={formData.dateOfExpense}
              onChange={(e) => setFormData((prev) => ({ ...prev, dateOfExpense: e.target.value }))}
              required
            />
          </div>

          <div>
            <Label htmlFor="description">Description</Label>
            <Textarea
              id="description"
              placeholder="Enter expense description..."
              value={formData.description}
              onChange={(e) => setFormData((prev) => ({ ...prev, description: e.target.value }))}
              required
            />
          </div>

          <div className="flex gap-2">
            <Button type="submit" disabled={isLoading} className="flex-1">
              {isLoading ? "Saving..." : expense ? "Update" : "Create"}
            </Button>
            {onCancel && (
              <Button type="button" variant="outline" onClick={onCancel}>
                Cancel
              </Button>
            )}
          </div>
        </form>
      </CardContent>
    </Card>
  )
}
