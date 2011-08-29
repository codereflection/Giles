# -*- encoding: utf-8 -*-

Gem::Specification.new do |s|
  s.name = "rubyzip"
  s.version = "0.9.4"

  s.required_rubygems_version = Gem::Requirement.new(">= 0") if s.respond_to? :required_rubygems_version=
  s.authors = ["Thomas Sondergaard"]
  s.date = "2009-12-31"
  s.email = "thomas(at)sondergaard.cc"
  s.homepage = "http://rubyzip.sourceforge.net/"
  s.require_paths = ["lib"]
  s.rubygems_version = "1.8.10"
  s.summary = "rubyzip is a ruby module for reading and writing zip files"

  if s.respond_to? :specification_version then
    s.specification_version = 2

    if Gem::Version.new(Gem::VERSION) >= Gem::Version.new('1.2.0') then
    else
    end
  else
  end
end
