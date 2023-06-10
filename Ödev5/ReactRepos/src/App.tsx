import todoLogo from './assets/todoLogo.svg';
import { useState } from 'react';

// custom hooks
import useLocalStorage from './hooks/useLocalStorage';

// custom components
import CustomForm from './components/CustomForm';
import EditForm from './components/EditForm';
import TaskList from './components/TaskList';

function App() {
    const [tasks, setTasks] = useLocalStorage('react-todo.tasks', []);
    const [previousFocusEl, setPreviousFocusEl] = useState<HTMLElement | null>(null);
    const [editedTask, setEditedTask] = useState<any>(null);
    const [isEditing, setIsEditing] = useState(false);

    const addTask = (task: any) => {
        const date = new Date();
        const dateString = date.toLocaleString();
        localStorage.setItem("createdTime", dateString);
        const newTask = { id: Date.now(), checked: false, name: task.name, time:dateString };
        setTasks((prevState: any) => [...prevState, newTask]);
    };

    const deleteTask = (id: number) => {
        setTasks((prevState: any) => prevState.filter((t: any) => t.id !== id));
    };

    const toggleTask = (id: number) => {
        setTasks((prevState: any) =>
            prevState.map((t: any) =>
                t.id === id ? { ...t, checked: !t.checked } : t
            )
        );
    };

    const updateTask = (task: any) => {
        setTasks((prevState: any) =>
            prevState.map((t: any) =>
                t.id === task.id ? { ...t, name: task.name } : t
            )
        );
        closeEditMode();
    };

    const closeEditMode = () => {
        setIsEditing(false);
        if (previousFocusEl) {
            previousFocusEl.focus();
        }
    };

    const enterEditMode = (task: any) => {
        setEditedTask(task);
        setIsEditing(true);
        setPreviousFocusEl(document.activeElement as HTMLElement);
    };

    return (
        <div className="container" >
            <header>
                <img src={ todoLogo } alt="Todo Logo"/>
            </header>
            {isEditing && (
                <EditForm
                    editedTask={editedTask}
                    updateTask={updateTask}
                    closeEditMode={closeEditMode}
                />
            )}
            <CustomForm addTask={addTask} />
            {tasks && (
                <TaskList
                    tasks={tasks}
                    deleteTask={deleteTask}
                    toggleTask={toggleTask}
                    enterEditMode={enterEditMode}
                />
            )}
        </div>

    );
}

export default App;
