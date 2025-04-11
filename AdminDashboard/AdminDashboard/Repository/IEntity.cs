namespace AdminDashboard.Repository
{
    public interface IEntity<Entity>
    {
        public List<Entity> getAll();
        public Entity getById<T>(T id);

        public void add(Entity entity);

        public void edit(Entity entity);

        public void delete<T>(T id);
    }
}
