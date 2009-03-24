using System;
using System.Collections.Generic;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace AutoMapper.UnitTests
{
	namespace MemberResolution
	{
		public class When_mapping_derived_classes : AutoMapperSpecBase
		{
			private DtoObject[] _result;

			public class ModelObject
			{
				public string BaseString { get; set; }
			}

			public class ModelSubObject : ModelObject
			{
				public string SubString { get; set; }
			}

			public class DtoObject
			{
				public string BaseString { get; set; }
			}

			public class DtoSubObject : DtoObject
			{
				public string SubString { get; set; }
			}

			protected override void Establish_context()
			{
				Mapper.Reset();

				var model = new[]
					{
						new ModelObject {BaseString = "Base1"},
						new ModelSubObject {BaseString = "Base2", SubString = "Sub2"}
					};

				Mapper
					.CreateMap<ModelObject, DtoObject>()
					.Include<ModelSubObject, DtoSubObject>();

				Mapper.CreateMap<ModelSubObject, DtoSubObject>();

				_result = (DtoObject[]) Mapper.Map(model, typeof (ModelObject[]), typeof (DtoObject[]));
			}

			[Test]
			public void Should_map_both_the_base_and_sub_objects()
			{
				_result.Length.ShouldEqual(2);
				_result[0].BaseString.ShouldEqual("Base1");
				_result[1].BaseString.ShouldEqual("Base2");
			}

			[Test]
			public void Should_map_to_the_correct_respective_dto_types()
			{
				_result[0].ShouldBeInstanceOfType(typeof (DtoObject));
				_result[1].ShouldBeInstanceOfType(typeof (DtoSubObject));
			}
		}

		public class When_mapping_derived_classes_from_intefaces_to_abstract : AutoMapperSpecBase
		{
			private DtoObject[] _result;

			public interface IModelObject
			{
				string BaseString { get; set; }
			}

			public class ModelSubObject : IModelObject
			{
				public string SubString { get; set; }
				public string BaseString { get; set; }
			}

			public abstract class DtoObject
			{
				public virtual string BaseString { get; set; }
			}

			public class DtoSubObject : DtoObject
			{
				public string SubString { get; set; }
			}

			protected override void Establish_context()
			{
				Mapper.Reset();

				var model = new IModelObject[]
					{
						new ModelSubObject {BaseString = "Base2", SubString = "Sub2"}
					};

				Mapper.CreateMap<IModelObject, DtoObject>()
					.Include<ModelSubObject, DtoSubObject>();

				Mapper.CreateMap<ModelSubObject, DtoSubObject>();

				_result = (DtoObject[]) Mapper.Map(model, typeof (IModelObject[]), typeof (DtoObject[]));
			}

			[Test]
			public void Should_map_both_the_base_and_sub_objects()
			{
				_result.Length.ShouldEqual(1);
				_result[0].BaseString.ShouldEqual("Base2");
			}

			[Test]
			public void Should_map_to_the_correct_respective_dto_types()
			{
				_result[0].ShouldBeInstanceOfType(typeof (DtoSubObject));
				((DtoSubObject) _result[0]).SubString.ShouldEqual("Sub2");
			}
		}

		public class When_mapping_derived_classes_as_property_of_top_object : AutoMapperSpecBase
		{
			private DtoModel _result;

			public class Model
			{
				public IModelObject Object { get; set; }
			}

			public interface IModelObject
			{
				string BaseString { get; set; }
			}

			public class ModelSubObject : IModelObject
			{
				public string SubString { get; set; }
				public string BaseString { get; set; }
			}

			public class DtoModel
			{
				public DtoObject Object { get; set; }
			}

			public abstract class DtoObject
			{
				public virtual string BaseString { get; set; }
			}

			public class DtoSubObject : DtoObject
			{
				public string SubString { get; set; }
			}

			protected override void Establish_context()
			{
				Mapper.CreateMap<Model, DtoModel>();

				Mapper.CreateMap<IModelObject, DtoObject>()
					.Include<ModelSubObject, DtoSubObject>();

				Mapper.CreateMap<ModelSubObject, DtoSubObject>();
			}

			[Test]
			public void Should_map_object_to_sub_object()
			{
				var model = new Model
					{
						Object = new ModelSubObject {BaseString = "Base2", SubString = "Sub2"}
					};

				_result = Mapper.Map<Model, DtoModel>(model);
				_result.Object.ShouldNotBeNull();
				_result.Object.ShouldBeInstanceOf<DtoSubObject>();
				_result.Object.ShouldBeInstanceOf<DtoSubObject>();
				_result.Object.BaseString.ShouldEqual("Base2");
				((DtoSubObject) _result.Object).SubString.ShouldEqual("Sub2");
			}
		}

		public class When_mapping_dto_with_only_properties : AutoMapperSpecBase
		{
			private ModelDto _result;

			public class ModelObject
			{
				public DateTime BaseDate { get; set; }
				public ModelSubObject Sub { get; set; }
				public ModelSubObject Sub2 { get; set; }
				public ModelSubObject SubWithExtraName { get; set; }
				public ModelSubObject SubMissing { get; set; }
			}

			public class ModelSubObject
			{
				public string ProperName { get; set; }
				public ModelSubSubObject SubSub { get; set; }
			}

			public class ModelSubSubObject
			{
				public string IAmACoolProperty { get; set; }
			}

			public class ModelDto
			{
				public DateTime BaseDate { get; set; }
				public DateTime BaseDate2 { get; set; }
				public string SubProperName { get; set; }
				public string Sub2ProperName { get; set; }
				public string SubWithExtraNameProperName { get; set; }
				public string SubSubSubIAmACoolProperty { get; set; }
				public string SubMissingSubSubIAmACoolProperty { get; set; }
			}

			protected override void Establish_context()
			{
				Mapper.Reset();

				var model = new ModelObject
					{
						BaseDate = new DateTime(2007, 4, 5),
						Sub = new ModelSubObject
							{
								ProperName = "Some name",
								SubSub = new ModelSubSubObject
									{
										IAmACoolProperty = "Cool daddy-o"
									}
							},
						Sub2 = new ModelSubObject
							{
								ProperName = "Sub 2 name"
							},
						SubWithExtraName = new ModelSubObject
							{
								ProperName = "Some other name"
							},
						SubMissing = new ModelSubObject
							{
								ProperName = "I have a missing sub sub object"
							}
					};

				Mapper.CreateMap<ModelObject, ModelDto>();
				_result = Mapper.Map<ModelObject, ModelDto>(model);
			}

			[Test]
			public void Should_map_item_in_first_level_of_hierarchy()
			{
				_result.BaseDate.ShouldEqual(new DateTime(2007, 4, 5));
			}

			[Test]
			public void Should_map_a_member_with_a_number()
			{
				_result.Sub2ProperName.ShouldEqual("Sub 2 name");
			}

			[Test]
			public void Should_map_item_in_second_level_of_hierarchy()
			{
				_result.SubProperName.ShouldEqual("Some name");
			}

			[Test]
			public void Should_map_item_with_more_items_in_property_name()
			{
				_result.SubWithExtraNameProperName.ShouldEqual("Some other name");
			}

			[Test]
			public void Should_map_item_in_any_level_of_depth_in_the_hierarchy()
			{
				_result.SubSubSubIAmACoolProperty.ShouldEqual("Cool daddy-o");
			}
		}

        public class When_mapping_dto_with_only_fields : AutoMapperSpecBase
        {
            private ModelDto _result;

            public class ModelObject
            {
                public DateTime BaseDate;
                public ModelSubObject Sub;
                public ModelSubObject Sub2;
                public ModelSubObject SubWithExtraName;
                public ModelSubObject SubMissing;
            }

            public class ModelSubObject
            {
                public string ProperName;
                public ModelSubSubObject SubSub;
            }

            public class ModelSubSubObject
            {
                public string IAmACoolProperty;
            }

            public class ModelDto
            {
                public DateTime BaseDate;
                public DateTime BaseDate2;
                public string SubProperName;
                public string Sub2ProperName;
                public string SubWithExtraNameProperName;
                public string SubSubSubIAmACoolProperty;
                public string SubMissingSubSubIAmACoolProperty;            }

            protected override void Establish_context()
            {
                Mapper.Reset();

                var model = new ModelObject
                {
                    BaseDate = new DateTime(2007, 4, 5),
                    Sub = new ModelSubObject
                    {
                        ProperName = "Some name",
                        SubSub = new ModelSubSubObject
                        {
                            IAmACoolProperty = "Cool daddy-o"
                        }
                    },
                    Sub2 = new ModelSubObject
                    {
                        ProperName = "Sub 2 name"
                    },
                    SubWithExtraName = new ModelSubObject
                    {
                        ProperName = "Some other name"
                    },
                    SubMissing = new ModelSubObject
                    {
                        ProperName = "I have a missing sub sub object"
                    }
                };

                Mapper.CreateMap<ModelObject, ModelDto>();
                _result = Mapper.Map<ModelObject, ModelDto>(model);
            }

            [Test]
            public void Should_map_item_in_first_level_of_hierarchy()
            {
                _result.BaseDate.ShouldEqual(new DateTime(2007, 4, 5));
            }

            [Test]
            public void Should_map_a_member_with_a_number()
            {
                _result.Sub2ProperName.ShouldEqual("Sub 2 name");
            }

            [Test]
            public void Should_map_item_in_second_level_of_hierarchy()
            {
                _result.SubProperName.ShouldEqual("Some name");
            }

            [Test]
            public void Should_map_item_with_more_items_in_property_name()
            {
                _result.SubWithExtraNameProperName.ShouldEqual("Some other name");
            }

            [Test]
            public void Should_map_item_in_any_level_of_depth_in_the_hierarchy()
            {
                _result.SubSubSubIAmACoolProperty.ShouldEqual("Cool daddy-o");
            }
        }

        public class When_mapping_dto_with_fields_and_properties : AutoMapperSpecBase
        {
            private ModelDto _result;

            public class ModelObject
            {
                public DateTime BaseDate { get; set;}
                public ModelSubObject Sub;
                public ModelSubObject Sub2 { get; set;}
                public ModelSubObject SubWithExtraName;
                public ModelSubObject SubMissing { get; set; }
            }

            public class ModelSubObject
            {
                public string ProperName { get; set;}
                public ModelSubSubObject SubSub;
            }

            public class ModelSubSubObject
            {
                public string IAmACoolProperty { get; set;}
            }

            public class ModelDto
            {
                public DateTime BaseDate;
                public DateTime BaseDate2 { get; set;}
                public string SubProperName;
                public string Sub2ProperName { get; set;}
                public string SubWithExtraNameProperName;
                public string SubSubSubIAmACoolProperty;
                public string SubMissingSubSubIAmACoolProperty { get; set;}
            }

            protected override void Establish_context()
            {
                Mapper.Reset();

                var model = new ModelObject
                {
                    BaseDate = new DateTime(2007, 4, 5),
                    Sub = new ModelSubObject
                    {
                        ProperName = "Some name",
                        SubSub = new ModelSubSubObject
                        {
                            IAmACoolProperty = "Cool daddy-o"
                        }
                    },
                    Sub2 = new ModelSubObject
                    {
                        ProperName = "Sub 2 name"
                    },
                    SubWithExtraName = new ModelSubObject
                    {
                        ProperName = "Some other name"
                    },
                    SubMissing = new ModelSubObject
                    {
                        ProperName = "I have a missing sub sub object"
                    }
                };

                Mapper.CreateMap<ModelObject, ModelDto>();
                _result = Mapper.Map<ModelObject, ModelDto>(model);
            }

            [Test]
            public void Should_map_item_in_first_level_of_hierarchy()
            {
                _result.BaseDate.ShouldEqual(new DateTime(2007, 4, 5));
            }

            [Test]
            public void Should_map_a_member_with_a_number()
            {
                _result.Sub2ProperName.ShouldEqual("Sub 2 name");
            }

            [Test]
            public void Should_map_item_in_second_level_of_hierarchy()
            {
                _result.SubProperName.ShouldEqual("Some name");
            }

            [Test]
            public void Should_map_item_with_more_items_in_property_name()
            {
                _result.SubWithExtraNameProperName.ShouldEqual("Some other name");
            }

            [Test]
            public void Should_map_item_in_any_level_of_depth_in_the_hierarchy()
            {
                _result.SubSubSubIAmACoolProperty.ShouldEqual("Cool daddy-o");
            }
        }

		public class When_ignoring_a_dto_property_during_configuration : AutoMapperSpecBase
		{
			private TypeMap[] _allTypeMaps;
			private Source _source;

			private class Source
			{
				public string Value { get; set; }
			}

			private class Destination
			{
				public bool Ignored
				{
					get { return true; }
				}

				public string Value { get; set; }
			}

			[Test]
			public void Should_not_report_it_as_unmapped()
			{
				Array.ForEach(_allTypeMaps, t => t.GetUnmappedPropertyNames().ShouldBeOfLength(0));
			}

			[Test]
			public void Should_map_successfully()
			{
				var destination = Mapper.Map<Source, Destination>(_source);
				destination.Value.ShouldEqual("foo");
				destination.Ignored.ShouldBeTrue();
			}

			[Test]
			public void Should_succeed_configration_check()
			{
				Mapper.AssertConfigurationIsValid();
			}

			protected override void Establish_context()
			{
				_source = new Source {Value = "foo"};
				Mapper.CreateMap<Source, Destination>()
					.ForMember(x => x.Ignored, opt => opt.Ignore());
				_allTypeMaps = Mapper.GetAllTypeMaps();
			}
		}

		public class When_mapping_dto_with_get_methods : AutoMapperSpecBase
		{
			private ModelDto _result;

			private class ModelObject
			{
				public string GetSomeCoolValue()
				{
					return "Cool value";
				}

				public ModelSubObject Sub { get; set; }
			}

			private class ModelSubObject
			{
				public string GetSomeOtherCoolValue()
				{
					return "Even cooler";
				}
			}

			private class ModelDto
			{
				public string SomeCoolValue { get; set; }
				public string SubSomeOtherCoolValue { get; set; }
			}

			protected override void Establish_context()
			{
				var model = new ModelObject
					{
						Sub = new ModelSubObject()
					};

				Mapper.CreateMap<ModelObject, ModelDto>();

				_result = Mapper.Map<ModelObject, ModelDto>(model);
			}

			[Test]
			public void Should_map_base_method_value()
			{
				_result.SomeCoolValue.ShouldEqual("Cool value");
			}

			[Test]
			public void Should_map_second_level_method_value_off_of_property()
			{
				_result.SubSomeOtherCoolValue.ShouldEqual("Even cooler");
			}
		}

		public class When_mapping_a_dto_with_names_matching_properties : AutoMapperSpecBase
		{
			private ModelDto _result;

			private class ModelObject
			{
				public string SomeCoolValue()
				{
					return "Cool value";
				}

				public ModelSubObject Sub { get; set; }
			}

			private class ModelSubObject
			{
				public string SomeOtherCoolValue()
				{
					return "Even cooler";
				}
			}

			private class ModelDto
			{
				public string SomeCoolValue { get; set; }
				public string SubSomeOtherCoolValue { get; set; }
			}

			protected override void Establish_context()
			{
				var model = new ModelObject
					{
						Sub = new ModelSubObject()
					};

				Mapper.CreateMap<ModelObject, ModelDto>();

				_result = Mapper.Map<ModelObject, ModelDto>(model);
			}

			[Test]
			public void Should_map_base_method_value()
			{
				_result.SomeCoolValue.ShouldEqual("Cool value");
			}

			[Test]
			public void Should_map_second_level_method_value_off_of_property()
			{
				_result.SubSomeOtherCoolValue.ShouldEqual("Even cooler");
			}
		}

		public class When_mapping_with_a_dto_subtype : AutoMapperSpecBase
		{
			private ModelDto _result;

			private class ModelObject
			{
				public ModelSubObject Sub { get; set; }
			}

			private class ModelSubObject
			{
				public string SomeValue { get; set; }
			}

			private class ModelDto
			{
				public ModelSubDto Sub { get; set; }
			}

			private class ModelSubDto
			{
				public string SomeValue { get; set; }
			}

			protected override void Establish_context()
			{
				Mapper.CreateMap<ModelObject, ModelDto>();
				Mapper.CreateMap<ModelSubObject, ModelSubDto>();

				var model = new ModelObject
					{
						Sub = new ModelSubObject
							{
								SomeValue = "Some value"
							}
					};

				_result = Mapper.Map<ModelObject, ModelDto>(model);
			}

			[Test]
			public void Should_map_the_model_sub_type_to_the_dto_sub_type()
			{
				_result.Sub.ShouldNotBeNull();
				_result.Sub.SomeValue.ShouldEqual("Some value");
			}
		}

		public class When_mapping_a_dto_with_a_set_only_property_and_a_get_method : AutoMapperSpecBase
		{
			private ModelDto _result;

			private class ModelDto
			{
				public int SomeValue { get; set; }
			}

			private class ModelObject
			{
				private int _someValue;

				public int SomeValue
				{
					set { _someValue = value; }
				}

				public int GetSomeValue()
				{
					return _someValue;
				}
			}

			protected override void Establish_context()
			{
				Mapper.CreateMap<ModelObject, ModelDto>();

				var model = new ModelObject();
				model.SomeValue = 46;

				_result = Mapper.Map<ModelObject, ModelDto>(model);
			}

			[Test]
			public void Should_map_the_get_method_to_the_dto()
			{
				_result.SomeValue.ShouldEqual(46);
			}
		}

		public class When_mapping_using_a_custom_member_mappings : AutoMapperSpecBase
		{
			private ModelDto _result;

			private class ModelObject
			{
				public int Blarg { get; set; }
				public string MoreBlarg { get; set; }

				public int SomeMethodToGetMoreBlarg()
				{
					return 45;
				}

				public string SomeValue { get; set; }
				public ModelSubObject SomeWeirdSubObject { get; set; }

				public string IAmSomeMethod()
				{
					return "I am some method";
				}
			}

			private class ModelSubObject
			{
				public int Narf { get; set; }
				public ModelSubSubObject SubSub { get; set; }

				public string SomeSubValue()
				{
					return "I am some sub value";
				}
			}

			private class ModelSubSubObject
			{
				public int Norf { get; set; }

				public string SomeSubSubValue()
				{
					return "I am some sub sub value";
				}
			}

			private class ModelDto
			{
				public int Splorg { get; set; }
				public string SomeValue { get; set; }
				public string SomeMethod { get; set; }
				public int SubNarf { get; set; }
				public string SubValue { get; set; }
				public int GrandChildInt { get; set; }
				public string GrandChildString { get; set; }
				public string BlargBucks { get; set; }
				public int BlargPlus3 { get; set; }
				public int BlargMinus2 { get; set; }
				public int MoreBlarg { get; set; }
			}

			protected override void Establish_context()
			{
				var model = new ModelObject
					{
						Blarg = 10,
						SomeValue = "Some value",
						SomeWeirdSubObject = new ModelSubObject
							{
								Narf = 5,
								SubSub = new ModelSubSubObject
									{
										Norf = 15
									}
							},
						MoreBlarg = "adsfdsaf"
					};
				Mapper
					.CreateMap<ModelObject, ModelDto>()
					.ForMember(dto => dto.Splorg, opt => opt.MapFrom(m => m.Blarg))
					.ForMember(dto => dto.SomeMethod, opt => opt.MapFrom(m => m.IAmSomeMethod()))
					.ForMember(dto => dto.SubNarf, opt => opt.MapFrom(m => m.SomeWeirdSubObject.Narf))
					.ForMember(dto => dto.SubValue, opt => opt.MapFrom(m => m.SomeWeirdSubObject.SomeSubValue()))
					.ForMember(dto => dto.GrandChildInt, opt => opt.MapFrom(m => m.SomeWeirdSubObject.SubSub.Norf))
					.ForMember(dto => dto.GrandChildString, opt => opt.MapFrom(m => m.SomeWeirdSubObject.SubSub.SomeSubSubValue()))
					.ForMember(dto => dto.MoreBlarg, opt => opt.MapFrom(m => m.SomeMethodToGetMoreBlarg()))
					.ForMember(dto => dto.BlargPlus3, opt => opt.MapFrom(m => m.Blarg.Plus(3)))
					.ForMember(dto => dto.BlargMinus2, opt => opt.MapFrom(m => m.Blarg - 2));

				_result = Mapper.Map<ModelObject, ModelDto>(model);
			}

			[Test]
			public void Should_preserve_the_existing_mapping()
			{
				_result.SomeValue.ShouldEqual("Some value");
			}

			[Test]
			public void Should_map_top_level_properties()
			{
				_result.Splorg.ShouldEqual(10);
			}

			[Test]
			public void Should_map_methods_results()
			{
				_result.SomeMethod.ShouldEqual("I am some method");
			}

			[Test]
			public void Should_map_children_properties()
			{
				_result.SubNarf.ShouldEqual(5);
			}

			[Test]
			public void Should_map_children_methods()
			{
				_result.SubValue.ShouldEqual("I am some sub value");
			}

			[Test]
			public void Should_map_grandchildren_properties()
			{
				_result.GrandChildInt.ShouldEqual(15);
			}

			[Test]
			public void Should_map_grandchildren_methods()
			{
				_result.GrandChildString.ShouldEqual("I am some sub sub value");
			}

			[Test]
			public void Should_map_blarg_plus_three_using_extension_method()
			{
				_result.BlargPlus3.ShouldEqual(13);
			}

			[Test]
			public void Should_map_blarg_minus_2_using_lambda()
			{
				_result.BlargMinus2.ShouldEqual(8);
			}

			[Test]
			public void Should_override_existing_matches_for_new_mappings()
			{
				_result.MoreBlarg.ShouldEqual(45);
			}
		}

		public class When_mapping_a_collection_to_a_more_type_specific_collection : AutoMapperSpecBase
		{
			private ModelDto _result;

			private class Model
			{
				public List<Item> Items { get; set; }
			}

			private class Item
			{
				public string Prop { get; set; }
			}

			private class SubItem : Item
			{
				public string SubProp { get; set; }
			}

			private class ModelDto
			{
				public SubItemDto[] Items { get; set; }
			}

			private class ItemDto
			{
				public string Prop { get; set; }
			}

			private class SubItemDto : ItemDto
			{
				public string SubProp { get; set; }
			}

			protected override void Establish_context()
			{
				Mapper.CreateMap<Model, ModelDto>();
				Mapper.CreateMap<Item, ItemDto>();
				Mapper.CreateMap<SubItem, SubItemDto>();

				var model = new Model
					{
						Items = new List<Item>
							{
								new SubItem
									{
										Prop = "value1",
										SubProp = "value2"
									}
							}
					};
				_result = Mapper.Map<Model, ModelDto>(model);
			}

			[Test]
			public void Should_map_correctly_if_all_types_map()
			{
				_result.Items[0].Prop.ShouldEqual("value1");
				_result.Items[0].SubProp.ShouldEqual("value2");
			}
		}
	
		public class When_mapping_to_a_top_level_camelCased_destination_member : SpecBase
		{
			private Destination _result;

			private class Source
			{
				public int SomeValueWithPascalName { get; set; }
			}

			private class Destination
			{
				public int someValueWithPascalName { get; set; }
			}

			protected override void Establish_context()
			{
				Mapper.CreateMap<Source, Destination>();
			}

			protected override void Because_of()
			{
				var source = new Source {SomeValueWithPascalName = 5};
				_result = Mapper.Map<Source, Destination>(source);
			}

			[Test]
			public void Should_match_to_PascalCased_source_member()
			{
				_result.someValueWithPascalName.ShouldEqual(5);
			}

			[Test]
			public void Should_pass_configuration_check()
			{
				Mapper.AssertConfigurationIsValid();
			}
		}

	}

	public static class MapFromExtensions
	{
		public static int Plus(this int left, int right)
		{
			return left + right;
		}
	}
}