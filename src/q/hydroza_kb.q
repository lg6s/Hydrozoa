tasks:([`u#tiseq:`symbol$()]oa:`boolean$();per:`long$();`s#obs:`long$();valve:`symbol$())
/ tiseq -> task identification sequence
/ oa -> action to open valve (true/false)
/ per -> period of this task (sec)
/ obs -> one example for a time when this task is executed (observation) (unix time)
/ valve -> the valve that will be opened  

jobs:([`u#jiseq:`symbol$()]t:`tasks$())
/ jiseq -> job identification sequence 
/ t -> a task of this job