﻿using System;
using Ideastrike.Nancy.Models;
using System.Linq;
using Nancy;

namespace Ideastrike.Nancy.Modules
{
    public class IdeaModule : NancyModule
    {
        public IdeaModule(IIdeaRepository ideas)
            : base("/idea")
        {
            Get["/{id}"] = parameters =>
                               {
                                   int id = parameters.id;
                                   Idea idea = ideas.GetIdea(id);
								   if (idea == null)
                                       return View["Shared/404"];

                                   return View["Idea/Index", idea];

                               };

            Post["/new"] = _ =>
            {
                var i = new Idea
                            {
                                Time = DateTime.UtcNow,
                                Title = Request.Form.Title,
                                Description = Request.Form.Description,
                            };

                ideas.AddIdea(i);

                return Response.AsRedirect("/idea/" + i.Id);
            };

            Get["/{id}/vote/{userid}"] = parameters =>
            {
                Idea idea = ideas.GetIdea(parameters.id);
                ideas.Vote(idea, parameters.userid, 1);

                return Response.AsJson(new
                                {
                                    Status = "OK",
                                    NewVotes = idea.Votes.Sum(v => v.Value)
                                });
            };

            Post["/{id}/comment"] = parameters =>
            {
                // TODO: when viewing an idea, can add a comment
                var idea = ideas.GetIdea(parameters.id);
                var comment = ideas.Comment(idea, parameters.userid, parameters.Content);

                return Response.AsJson(new
                {
                    Status = "OK",
                    NewComment = comment
                });
            };

            Get["/{id}/delete"] = parameters =>
            {
                int id = parameters.id;
                ideas.DeleteIdea(id);
                return string.Format("Deleted Item {0}", id);
            };
        }
    }
}