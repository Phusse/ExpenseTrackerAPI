"use client"

import { useState, useEffect } from "react"
import { Button } from "@/components/ui/button"
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger } from "@/components/ui/dialog"
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogTrigger,
} from "@/components/ui/alert-dialog"
import { ExpenseForm } from "@/components/expense-form"
import { ExpenseList } from "@/components/expense-list"
import { ExpenseFiltersComponent } from "@/components/expense-filters"
import { ExpenseDashboard } from "@/components/expense-dashboard"
import { ExpenseAPI } from "@/lib/expense-api"
import type { Expense, ExpenseFilters } from "@/types/expense"
import { toast } from "@/hooks/use-toast"
import { Plus, Trash2, RefreshCw } from "lucide-react"

export default function ExpenseTracker() {
  const [expenses, setExpenses] = useState<Expense[]>([])
  const [filteredExpenses, setFilteredExpenses] = useState<Expense[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [isAddDialogOpen, setIsAddDialogOpen] = useState(false)
  const [activeFilters, setActiveFilters] = useState<ExpenseFilters>({})

  const loadExpenses = async () => {
    setIsLoading(true)
    try {
      const data = await ExpenseAPI.getAllExpenses()
      setExpenses(data)
      setFilteredExpenses(data)
    } catch (error) {
      toast({
        title: "Error",
        description: "Failed to load expenses.",
        variant: "destructive",
      })
    } finally {
      setIsLoading(false)
    }
  }

  const handleFilter = async (filters: ExpenseFilters) => {
    try {
      setActiveFilters(filters)
      const response = await ExpenseAPI.getFilteredExpenses(filters)
      if (response.success && response.data) {
        setFilteredExpenses(response.data)
      } else {
        setFilteredExpenses([])
        toast({
          title: "No Results",
          description: response.message || "No expenses match your filters.",
        })
      }
    } catch (error) {
      toast({
        title: "Error",
        description: "Failed to filter expenses.",
        variant: "destructive",
      })
    }
  }

  const handleClearFilters = () => {
    setActiveFilters({})
    setFilteredExpenses(expenses)
  }

  const handleDeleteAll = async () => {
    try {
      await ExpenseAPI.deleteAllExpenses()
      toast({ title: "Success", description: "All expenses deleted successfully!" })
      loadExpenses()
    } catch (error) {
      toast({
        title: "Error",
        description: "Failed to delete all expenses.",
        variant: "destructive",
      })
    }
  }

  const handleAddSuccess = () => {
    setIsAddDialogOpen(false)
    loadExpenses()
  }

  useEffect(() => {
    loadExpenses()
  }, [])

  const hasActiveFilters = Object.values(activeFilters).some(
    (value) => value !== undefined && value !== "" && value !== null,
  )

  return (
    <div className="min-h-screen bg-background">
      <div className="container mx-auto py-8 space-y-8">
        <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
          <div>
            <h1 className="text-3xl font-bold">Expense Tracker</h1>
            <p className="text-muted-foreground">Manage and track your expenses efficiently</p>
          </div>

          <div className="flex gap-2">
            <Button onClick={loadExpenses} variant="outline" size="sm">
              <RefreshCw className="w-4 h-4 mr-2" />
              Refresh
            </Button>

            <Dialog open={isAddDialogOpen} onOpenChange={setIsAddDialogOpen}>
              <DialogTrigger asChild>
                <Button>
                  <Plus className="w-4 h-4 mr-2" />
                  Add Expense
                </Button>
              </DialogTrigger>
              <DialogContent>
                <DialogHeader>
                  <DialogTitle>Add New Expense</DialogTitle>
                </DialogHeader>
                <ExpenseForm onSuccess={handleAddSuccess} onCancel={() => setIsAddDialogOpen(false)} />
              </DialogContent>
            </Dialog>

            {expenses.length > 0 && (
              <AlertDialog>
                <AlertDialogTrigger asChild>
                  <Button variant="destructive" size="sm">
                    <Trash2 className="w-4 h-4 mr-2" />
                    Delete All
                  </Button>
                </AlertDialogTrigger>
                <AlertDialogContent>
                  <AlertDialogHeader>
                    <AlertDialogTitle>Delete All Expenses</AlertDialogTitle>
                    <AlertDialogDescription>
                      Are you sure you want to delete all expenses? This action cannot be undone.
                    </AlertDialogDescription>
                  </AlertDialogHeader>
                  <AlertDialogFooter>
                    <AlertDialogCancel>Cancel</AlertDialogCancel>
                    <AlertDialogAction onClick={handleDeleteAll}>Delete All</AlertDialogAction>
                  </AlertDialogFooter>
                </AlertDialogContent>
              </AlertDialog>
            )}
          </div>
        </div>

        <ExpenseDashboard />

        <ExpenseFiltersComponent onFilter={handleFilter} onClear={handleClearFilters} />

        {isLoading ? (
          <div className="flex justify-center py-8">
            <div className="text-muted-foreground">Loading expenses...</div>
          </div>
        ) : (
          <ExpenseList expenses={hasActiveFilters ? filteredExpenses : expenses} onExpenseUpdated={loadExpenses} />
        )}
      </div>
    </div>
  )
}
