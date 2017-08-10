﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017  Dirk Stolle

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Firefox, release channel
    /// </summary>
    public class Firefox : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for Firefox class
        /// </summary>
        private static NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Firefox).FullName);



        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Firefox(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
            }
            if (!d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// gets a dictionary with the known checksums for the installers (key: language, value: checksum)
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/55.0/SHA512SUMS
            var result = new Dictionary<string, string>();
            result.Add("ach", "1b8381da4eb3b404ce3e8f68f9be34f1376207c4fa4945e1478137ce88f1835084665e72a48cd8cd3a6a52ffa05ca50ec40c52d5507a7c40320f54dddf040e43");
            result.Add("af", "c96aa69b80794518ab91c0618f58f41452ca5685b59b53d1184e42899d76023c5a22545090f5dec735739baa816d336aa0dfebc889dc4abf4c6cff03e1e4b477");
            result.Add("an", "f7556abe0864cb0f05b89eec318a0368f62c652fd6cb672c30e18c74899e2a24c9c677e7a9eb9ff85225ba40e905a622eec9b48b6f14242559ab4da591dcb0ad");
            result.Add("ar", "aed5e5dda71b34ca0b66d3f1166af925793d7dab8d6d883739f99b16c795c5f19bcfcc1a39999d7c7fac7920bc84f3b684d5a4de5a6e5ec47cff0324bb590113");
            result.Add("as", "5d61edb95a09f3693124b0a93608fa39f91250676fc54b69276b26b5bf0c393575f1df78a22c28ffc8c4029d70b25e86572a6ce3cb0c70df6da2637c82d9dd31");
            result.Add("ast", "554046334aef6263d2c4b9c760c700e816ede31cc1c837762a7d9e318e75df91e6e7d425448b4ac572fb9f107fb93b6a73731bb79a27683b55a3fb49d09b29f6");
            result.Add("az", "ac7965cb5b02bcfd6514619f2c11ea2817d967a59c2ae655baa3d6c2600001f9c9cc150aaa3f96c74a14c4fbabd630930c9b999486b0b371c6e1e4d8b1aa723c");
            result.Add("be", "2c0cf3c10331451b9d19713496d071ac95fed66efbf4e6ab95d910e6fcdd1ff6ef38a22eb3c224237fb385b8dc7266426c73f62078bd1f8fb72847c2644057ae");
            result.Add("bg", "c66fcd1f86e25c6eaa3d9d6dfaabeff15ced5e5a12f053ed534da31163452c242d95f985e07678c2abf0e130b0cf6c028c5376bc607ddc87260aeed1ff84e5ab");
            result.Add("bn-BD", "13f54168b7512406b4add2766db87aac28cc2932be53e8ad2d17a0062a46f937064366b28e596513273f3f3efbf7f1430f3e5e257ddd38f91b6beff0379f10af");
            result.Add("bn-IN", "79200082a6e733a3c235bb08b46fe64abc10128fd8531fbf19b3db11acd1106e707ffee021f3b63b632b8d0d60f4fa3f40f39b1dceb6c8281d0c0e7707f15db8");
            result.Add("br", "d8092593e818ce7c8fbb4061ade094e0372d4ccb616bdb8a5ca40553d1be7889af9d781eab376d4aa9e0e2df2b8e134d1dbd09866353742edeeec0f79522afa6");
            result.Add("bs", "d290ae37e83bc2776fbac611cee9f2bbddd16abebec6043ae193eb7702b865c53da878b24ae148d73cfc3ede20892790cfa95c825f43195bc64390dc17cb57e7");
            result.Add("ca", "0c59a355999d0f78698fd52ea28fb812f46e6025d030189e481859bfd839b389f2c6b103397087414b3502a7b0fcca7ceb0c3ca5b7e6983443c55fc9730a6204");
            result.Add("cak", "a6c40646a0284a428d20ad0480ba8cbb79a1eec1e25ad64dbc21e162bdb4eb8784c0aa3ac30406a06d27f460e7287bc6f7b6f328bc0f5bf6b0a26dba9ba61ca9");
            result.Add("cs", "32bf27a9b46618de5ca492fb9fc85e5ec6aa7d28cdee83276dbe065156b930bbcc0b47d8403c8bc7fa9eab9d5df4b69a19bed95bb767670453abc37c0c709502");
            result.Add("cy", "06f04049d856edadd2c50626dc573d6c4a8268119c9b54775cae45d45ef8d0f1effedaa979cf3a2956761e977af82e145857a84e260b1a4efc5a1affd1bd7373");
            result.Add("da", "99c85234ed5f14a4b2680ae63e7eaa468ab8046797e319504082de12ec96a3a7d1b8ea96419d930c72a4bad4899fb8ee9a105e60cb38daf5ba75895df7f85e9b");
            result.Add("de", "384d5822b34c03481427ec5e20d76f0cb2648eebc36f54257f414a5a6fff963030c8969217027e9d1c78728724976a3a044bece2ef0cc421b166cd5952fe2126");
            result.Add("dsb", "511114583abdcc61eec7c9b88bf49f04e01dd2182cba38aba3c06226c2c048080907fe1a6c94f240423a841be3cfb8fa9242f20d6886d0d5c5d8a2e4abe17ae9");
            result.Add("el", "451e8ed88d11967c914add6038775da7b2ebf216f4cfab45a0e0291ed9f848b7ba57a3296a820c72cf1b312d8e9d091bcfb917e5de3749d02438a20dd912ed88");
            result.Add("en-GB", "7ae1b791462ef617b9cc2a20c85b8829272a90d655a3e71b912df898614feb86bff95fa84814733f39942e5544643ed4950485dc57e8eaf117d11d0c0c33cba2");
            result.Add("en-US", "54d225b30be4525f56d5941c8169346a9673d69d5b1c915948b02baf9aebba82c165265e9a8cf4e3f9879df7ae1eb8546737af5446c7ad59abd25af712185b9e");
            result.Add("en-ZA", "6dc979b2f211dbebcafc3d493041f2d218b14e02310fadb61e2c7d0a9b24064c40250cc13006f85f5c6e797f853725a3217e3452f5c70f4d3434f2a90384ced2");
            result.Add("eo", "8d8a35766045b284cf7d16b023d555759d4c8816f5b33dfbaa886c2d4530a89c16bab65de175134952a862e5430f6000622a414d090ebdfa04cce322f7f87a77");
            result.Add("es-AR", "d520541711472806951b8765f67716a404a3fc7e285b152d15ccf4b60f812bf60036f82a69ffa50a454e3c24bf4787e90ed7464fc133059b1855d4f466c9deeb");
            result.Add("es-CL", "c979d5e27807dc9ebf4c4a9416514eae330adfc78b449a0d33f4d36a57a48351ff251c7c51a879cbf99a7b270c46b20c0cf5b18129f9c67fd9f740d77fe0a58d");
            result.Add("es-ES", "1746368b03c8075cb768a5cf71acab98b6d711e7f348d6b71e14d26934e34256056893bd06f716d4658a8d92769b27ec7fabcb33b78d7708a74716e110133ca2");
            result.Add("es-MX", "1d2d753956c028dc255befb0c4e36fbe96c332a0d9549df56e13839e6118e8c0e1fde1c618d2132654a7f533f1e1c01c18c7b2f3abea7d0bfa757beddf26641c");
            result.Add("et", "f554b82a0b74f2e9338e7e5e34b3fee7731bc4a11868fc42a6ded43dabed62985ad43503215490fe8137f18ffff7a7740047ea3d9029d543ecd3fa0d47df56b1");
            result.Add("eu", "fe516460146656f0ceb87eee3922d682c7d10f439bb875db3214b9bd668b3387aefd069a8c0b602e89b6b3c75adeecf2083b1f373408ae154a3baee9393092ec");
            result.Add("fa", "845ac8be246947a1603d95f719945bd80edff801853452d61f976cfe691660deb791fb4a66e72f431ff075bdce6e8c4a8361fbdce40585dbbcc204673fa393cc");
            result.Add("ff", "8b3c62784b92bebad21247c7b25dc373463daaf9ed9626cb1772ef828f00a5c80543b6cca428c6c50069b9ab5da7899421cf337f9dc75bd3db354b80da99bd34");
            result.Add("fi", "d502f3a3a59039f5d615aecd1df0614ff2487e417fb2ae0514932472ed5d17e330924f1132dda38363537ed0a1ca27dc660a951370469e35bc42b0615855c373");
            result.Add("fr", "38907dc4ef572fdfb97babb7e953cdb11a89aa4f5064aab7f6e28522ccd848c0a8be53b16b67fa3b0a5563f7e6fb65612e409c85041a7ddd71b5065dd6870cf9");
            result.Add("fy-NL", "c1de3d09abe263001151fe9429e4bd8084d37ee1d884ccc1c8d0b9b6fe896cb9c9a4757e739ace70720f02d30b0aad58dbc2fa820144038a6bcbc1eaa251612c");
            result.Add("ga-IE", "5aa42fe68bed93c4b632d5ea7790988adca9bf2eb1d6c8297d28206ba36f17a27bc27265f121a63a3315f06deab118a0a57b17823a0110bb1d139d23bf45274c");
            result.Add("gd", "2bfa2b4becb210058f5697cec0688ab7c6740d2de099a25d94579efe5cd462e5657f85158b586a8484617eecc297c22957b391e78fcd6788e39cd0e79f5cc796");
            result.Add("gl", "f259b8f1da10e3d6ab162fd12c7fdcd4fe8821adfb6b6e38de608bc5dd425ceee93e90eba798a06e44c7ebcc6668a690e27395e27ffcf2c33f8263dce2e7c412");
            result.Add("gn", "ad4a361cd3105c0b748a46df0697c8e01080a2e56f08ea7eaed53507dc902448a2de17151958b0e6acf5447f31d72db08630f833879481877f441b161ea3fc62");
            result.Add("gu-IN", "17491943d15df347700b2d271630f03d6321f91addf7960679bd019115c0c148ab2447451aee33907ae3568911cd5d7a3d38ce0f33d357a76c7aa2fc20d1af7e");
            result.Add("he", "c1cc46ab0051c91d032b40e65e9ddd464dbeb6f2588ca923ffa08d75aac7f3c7a924f65d8840a8f3741816ca66bb8c296ef0e2ccf2cfaccc24abfc5d7dea9544");
            result.Add("hi-IN", "499a65e0bca0d1ca41fa98a410115a3b39bc2d082078e394cf107082e760ca910f48d56d1bffaa65cca1246d9dc8c4117489bf1818015ae1bc96599e2ef9fe9f");
            result.Add("hr", "af55c5d41c268b43d1ec031716f1276e755c1dcd5e944748d0b94247b4e5733a3b3bc9a6c60bf611d22ceaedf8455e0b17b4740676b4d9ee770f06e16cc952a7");
            result.Add("hsb", "f58af48e7b0919ebd728d828ec7df58c2dcfa412751e19186222158b76ff36665abdcdaa6ccf283f44d0728bd51e2ce4b6945c1e3eed9a056fd6ae08194791e7");
            result.Add("hu", "d9a5c06e8430df98ff36dbba80db345fc356fb3326f7a744a1416c1949cdfd3c0ddf655cf7750a0c855b147682555172b261287b9494d5f499c3736bea35e39e");
            result.Add("hy-AM", "9cd9e89bd374b3e418db3ce1662c283cb258dc447a63bdc26aa72e6550eb8a2e344b60b3aba6899e94bf97af4fcb23125723deda7a3f01827e208025a189f47d");
            result.Add("id", "4cc5ed38cbda4d6bfb62e39acfca425c0071efcc8b3e6aeac68e1117d2eec425717abd5e065f79a4c49886ca15341baa02e774f5d2d2798a8df855eb41a45a6f");
            result.Add("is", "75fef65ea3c5d00998cfca383cad8cc1f3c71e5bc3d9f6816b000e20b30265a5d21edbc02fb830d49422a09bad4e581500c8308ee2b408e65c705105de20b92d");
            result.Add("it", "cd506c5947f3876ead02742c78124a7a070242879bf9d6266a70058a8820d270cc5ae31c29ee780d6fee4b820afe9a197dba017f40a87fb87f0d9ff4a8a5b5eb");
            result.Add("ja", "635e151efe79bf05a1cc4a44b8b6532e7dd52d994d36abe358a8bcbd5caab0524a13098343b576b7f2746a67f02f3075fa0f7b3f7a1493deca794b8ad5346838");
            result.Add("ka", "c85815c840d6917292a0905dacd7ee0868b1f32a32215134f6d6f6c4421fa3390ac52e84f9cec08dd65562afb14998762abc56fd5db3ad05907c512edaf2c06f");
            result.Add("kab", "49c65db58719180acb2a73c1fd81d0907ec1f6becb56cb4355dfbf0f7e9d92c40e7dd430ce697223b128813a3e53f9b723d9cf314be9b836d0e71cf1002fa132");
            result.Add("kk", "fdb475de91d9f1c066dc23a47a117ea30ae806f3537bd60a7aabcdbdc4ec586df835fa2999792ba81085aa1b29cc11b8320635a9fe4a0b5f14bd3717fb53e052");
            result.Add("km", "fc40ea9ad2b6400d4ee88b03a486c76be67e7eac2fa04864006d4422e0f9ff24402df2194a84f06e37146ae0ed09435862ddfe1174a906b6e1c0f098f44a4323");
            result.Add("kn", "0d8e25060a4949f6c212bb07fc7f1d9a90cd73c79d5336a2e446f03103976b78a467869f404a6d92eedc7cdfed894317b628c81f807d7e71f5f9da7fad5273d9");
            result.Add("ko", "becd86b75afda932b764fdee37d28099a440367653fa51157fa18e6613c7b4e39ede63c87bbb47e39332da207930c982d7930040513d95b366d2ea3ef5f4ba2e");
            result.Add("lij", "2bee76bfcafc9dcc362d7a7ab72489d59d9fb935cae0bac28db9af1f54a199b9ef896f00b8e595c30bc3969b51fe186a3f6dead0339485f1768d4ea99934d74f");
            result.Add("lt", "9fdd585a3886cce97fbfd5e49e8e5725a7d569baf6cccb442465b872c3e0284392bb27fb90b00eb696783e0ec5f69c2e6ed64acabfbbfa8f49e9b7297ed357ba");
            result.Add("lv", "a414fe85c88552f8a10290da96e6cce6a5d047cc4e2f89f1129315a859ac9e7382c40fed7246a4bafbaf94d42b4479688564f26f653f1688879c173afdbf79f6");
            result.Add("mai", "e0738f69351ba093438e6be8ee72c44946538d0f33e1c6c89c0a50382ab5b8456d705ada7438c05ccf63f52178e66cd6d2e65af78d3d6264c7ac9544a39138e5");
            result.Add("mk", "06a248083844a52e7cd262c41039d5d557744d117d38c35e67d9201c2bc8498ece103bd45a55bf3f9b2849669cbbcad0efc7533f6d1d831b566d67d0bd5cf393");
            result.Add("ml", "09e26c182e5ef6dac03e17d5584d2e95b3e4be6267862f878372807eaaa648f28e2b7d4ef3d631490157979ccf6819a1ef970fb2fbed6e8226bee32b77e1dc29");
            result.Add("mr", "4f2239cc9a68e3f6a65421d9efcbf244b07a3e27cd6006f1a25dbf2105622d9ea31e7a6b2799a800efdd15ee171001f379fb2559a40f323d94073baee638a58d");
            result.Add("ms", "840443d05550dd102487c4476b165d2246a8053276ea0e4f245ea2446ea99fe3e9228fdaf5314d071beaef931e642d694ea4d036c5c9d91c7caaaa8c498b8152");
            result.Add("my", "2a72f3cf6770b172dc76d3b9063dafb37548e726d92ebc0fab463e1d5ab7ce6c9bca90cb048acb8f77a30671c1821a3c2934aee6499f7da1cd100ef136129fa0");
            result.Add("nb-NO", "412e3832800c1acab3316eaa1fd5c3ea33c996ab9163d978ac4ae653ec91711fd3c219feceefb1e5c8a838d564af989f52f995e40bd517f3bbc4dbc3fd0f28da");
            result.Add("nl", "0bbcb41d0c141f4c6ae42ed3e0f4bc173a7799e1cbe624c8d04bd059b1fb28cb8d9add375af0f2a4fa23792b7171573dbbd86625de56f5974ff2e7e5bb414b4c");
            result.Add("nn-NO", "2af8ea6edee43e8a5b74b4622e84b11906aa1cfc9ed92aba305b2af1b48e1f2a006b0f6bb5e34c1009426526525729f6aaa24645be1374a28157f15eaa5348ad");
            result.Add("or", "34a09ae2938ae1b919f6d6bd7e246a78a700644f7f57189a2c7c80ebc5a6faeac6f64eaf200df1dca4da7eca919243cf7ca3a902b9e6a3adeb34268852a0cc2c");
            result.Add("pa-IN", "8d3cc08bd3e3c78df6b383578f35566fe9756232e1c5a457bc591d6c724c9767156aef7912e7e5db38f463a00e864237d8d8a23debbe81e3de1f22bba4f9099b");
            result.Add("pl", "b82962dcdc5e76edb5d1daddb32cac8e4bb1ac5c8f1db4e96d9ec8f7784cd6bace0425858ff4420e4b55d2a2f2c18bd53d0ec3492275d32b6637446e38d0dad1");
            result.Add("pt-BR", "abf4fd249f546fc01e2a91a30770cee0c9d6e722b3c937278fff6d0a41b46ccb0207da6f2bc17d60df447ae4936a1e691812853e6ca5895ea6fed723a6ec71c6");
            result.Add("pt-PT", "5f0630178e6f290db43ce1ee47f5d09de1758f89c46b189d78504ab589d5c28c76e21259f288f017fe22f451b5257c74fcbcfe6fb805544851d60e3b3d3b803a");
            result.Add("rm", "0b2a9904a5b4b333f05866f24c899eab97c4fbcec310e014071e3246f99aa085136439079612edd6e6dca94f1a073758d2a7fb050a427101d07b1a25ded862c7");
            result.Add("ro", "fc9b8007e1ea78a4924a9797e17785cb78de69767724a3cdf0abae2ba4da7cbc412df7de6ce4dbe78f80d8a874084cba462828f622cb16f8483ca3882bc0107d");
            result.Add("ru", "c83157fa568360e51dd8c8b9e236f46b370e547884ab8067d6ed455dfb16f0bfdf0c21d39feee1effabfc379f72c8a520c33952303acebc7195903e3f5904bdd");
            result.Add("si", "b2360ae8bbf626ac7305c255ea453fde71b9b13b8a7cc04b39732e2ec2c5a2a69654cbc8daba4dc59bfcdaf25e4640b9ac507bbaee4c796ff853b7b7b64d0db0");
            result.Add("sk", "fa18d6dbcdd1da5818cbccca284ed8529d90162300a9478f30b59635a045341a864165dde418075b570a5fb65f84d2bc4635eda45cb204c7e30930b751ba0a30");
            result.Add("sl", "104667c3a51d3e83bd7735c32f7ac74d0e9f43af7bd447d22550c24c4b74273783535035d594029044686419c8913904b3381b0b9967d6e0a7003f007e2f431a");
            result.Add("son", "ce35a3aef474cb336c0f158577603c746bf0b0959677562d5dfe92435834d528dbcc90ad4b6868961f87f4c38120dfd6e30f323c00399d6d25263d43115cecad");
            result.Add("sq", "fbb5b2c4a72d739e260984e8355f14a84827e1ea02515eac1c3f837eac43d1ff8bcc2fb5a37ff8af10c80fe84dd9b1b7f3628ac3d5d57ef3c97598f7d0d18352");
            result.Add("sr", "c2ecf7d07de6eca8745afd4eadc82f6f0d90d3ec76f5e1c5e83e0a292e90c58093162483d57344e5f4e431813d2672643700b1be8cdeb0fe2ffee2fa31820e99");
            result.Add("sv-SE", "03c45c12004864f55794e04e8236e092f80002b7a6c4f447b4b4f7eb8d63a9b872f7ebd0447d3d7e84a63d8144a758d9f75ccbc88374939c3e4bfd6fae44f6a3");
            result.Add("ta", "d9506c3e7fb41aebad9e49cbd88180a234ee4e61e93796a119660519bfd3a11626d47677d14ba07c6a17d344b72812971d71e60b59ea2be2dc4a8d832c883ab1");
            result.Add("te", "531dfdb2711c4e396ed57faf8c44366392471c7f5f0dcf684ca792884c664a2b31add4fa00cdb150c567601dc5e0a2eee3d5dfcd16c481f1cb44a7fcd07dc1ab");
            result.Add("th", "3e636f72106a143c8bb6d96df1ef3750ab38b18d0987a1e055c8a8cc3056e366b21cb29d4bfc725d2958892e21cbd1b6d2d5b4b574483624be09c78c87653a60");
            result.Add("tr", "41d1e58d1f7faa5552c32a25c2e308608e513ebf70cd9262f84388f41122004425150e87d706ff81b379c491c16e2613f0f82403aeecf6c05d3b8d0804c27581");
            result.Add("uk", "5185fc882d6360d5121907a288809ba467f1a6c30593855646af064b84bd4735530ff85c4c180afb06f95de0e17b04f9f6ea7e1cbc00d87a3bbcc5678da8eec1");
            result.Add("ur", "8908b5266f2539865e1eae5a3635e234e855c5192d75683f3d7a4bfcfd3ba0f6bab14649241327915325ff0df266e0078a2ff7d1a037c742fa6579f5f921fc82");
            result.Add("uz", "ab2a3e87d9d3759a7764966441ff9baedd3d2c8c66b7415b55eda1d893c31e0ffd6901042b4351075c6da514f5175d96f2e6ed028ac86da6ef4dfcddf0c9f137");
            result.Add("vi", "ce74e03a1a1552bfe525bf196000f0031fce8ac16a3b8cefb5db7d6e8517406079d137231bfd51370bf54739e40b8bee269974b26af78fbf08d3138e4644e6e3");
            result.Add("xh", "1217b17c1299ca4665b21db0f28d76bc1b14feb41c261587232053618bc806e34ac0f503e804aaf44e40f874da22eaf6bf8983e98617b3c17574bc6853eb277a");
            result.Add("zh-CN", "3be52eb87061564e9bbd23fc5fc4f5b3a71dba845f402cfb10b438794d5118dbda6d50f90a23bbb31dc83f912fdddf77859276d44f260f8e691d3c45252769df");
            result.Add("zh-TW", "9635fa2984119f75f599f2c4d2a4486fed140a3298aa7652b3fc7e796fea99422cecb2c5b4f6099c4ef38b4e9de660bd4ef2efe9203e9ff87af1bdfeb6c9a243");

            return result;
        }


        /// <summary>
        /// gets a dictionary with the known checksums for the installers (key: language, value: checksum)
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/55.0/SHA512SUMS
            var result = new Dictionary<string, string>();
            result.Add("ach", "6079a334483031c080881f2bcb6c7fe13056a4bc9ea5d371830c552b794c5a4b68faa97b6907d9139075251db2a4b8d21ba9d2b5335d10d69caad89894f7969b");
            result.Add("af", "4d60ca7e2b2d3dbe95f15addcfe4a68f58d2f956a577c405f2ea1be31905f40fcb4d799ff870b0c79d843001576f057bec42e879cb9e67215689ecaabe4c22f8");
            result.Add("an", "c5f6ae35a60d2fcc2b1284c9a439114e2a2ca36a804c578a4b665f4de663f1b02f32c7e51a3602f36ea45097f422c71d7474daf435835ababc1313b117e658e2");
            result.Add("ar", "94df1f3ea6e6451ec2de14648b340bed194e5dc00692ca4003fb009997772c4b7368ba884654f27ecfe80ccae28933a9f845bfa63893ed723d7b68040422c611");
            result.Add("as", "b347923ecd687ac0b72621057f75b4b74ec429a4c8855407d7bd18dcbf21ac4d60ff834cd5bee379f8a89109470f4171ef0874fc717c70562ec18e5bf79b7548");
            result.Add("ast", "f09c7e23486d9ad976561fb09b87ade4b0fc96bfac1324025d83391c19b649226902832aa076818d04061c11f0f52d711554034821c2d8ba2a3613924f25f914");
            result.Add("az", "591f5434e0e08908ce9c19f71b8dfa493397a7605bdc2e36f99969b98ea4538c9c8a98ef4d016b6da84dfe1a1083385aabecc1ef9d2be101c078582d9c244022");
            result.Add("be", "c303a04d836f62846eba549149d136019963cbaa482edeb19ec01a9d698dda95b359becc14d5bd158d1e967efd944498d8d30ec5c5397688d3273e3b2deed626");
            result.Add("bg", "26cc7b9604c58286797484a539eee849cd0ee087e234a014ca4212649fdcd5730ed33fbba4dd26502b832d9d07688430cfc0dc6226e6209b6a85f2d1d5e12d98");
            result.Add("bn-BD", "d7b2bcfe9f9585b81f968ad4366e1131dfed57603237a863f1f67546cb7535d7aee21a27c2fdc9468a1f4705815bd20a562513163a5c5aa629dd5d66c19dc5ad");
            result.Add("bn-IN", "17b27bd7bc9eb3049f222e8aca430ca3bbbc936a70ff6b2c846926a8ceafd65ef0ac0326cc5c46390098c280948bafb7aa49b98e31866119aacb01590e35d056");
            result.Add("br", "e11e15d8b8b30afa2a90e0d9022b86a30db04782b9938b30817da724fb5fc2e39d574ba20ec476593362cbc60c4ab1a6526f6bb0b61b5ea0da3b942dfdaa2216");
            result.Add("bs", "b15f750a43ca8de8cc315a1c6add1167366d8036943947c0732cfbaf74f3f4113cfc4fb3876af1757d73dcf30c4d21e0a9377b8f00d10d10f739080cff3206ee");
            result.Add("ca", "1c81933b7bd8220046e0da29624ab130c89960c238024045dab2bed2971a9dc4745bedc24d4850292f4039504fb8a01ed42eb2cf825f94a6b5ea4edc878eaca8");
            result.Add("cak", "06a7593b2ae36be53d8e8c328e72b396f3d0d39bf33d4ce55f38e309ebafe4026ac81ec28a56970941383f6de745c8812bf18c02180fdb81e026a88d9cd3a835");
            result.Add("cs", "439fe7c175a7fcef9db41856114e7556e717b709e3ecc2561c30ef4d363ef88f4ccdef0bbbd83e6fcaa39fd3d5d63bcab9134c29e403aa1754a7029d6e21d213");
            result.Add("cy", "3a661330e21cd638fead5143903432765261dfe5a0fa75a15a9eea1304e076681cc23b5cca7d4ed25c8d4f7b33b3776ffd9fc346aa9c00cd0e13c2a98ac4042f");
            result.Add("da", "d0e0adf5a20c7f67cf7c95eacbf3b26b996662fcb810db97d1fe6c6c671d5d303ba6a6c12793c72251af2cf766b88ea8d315c243220dfbd4b4a45b74fb004d18");
            result.Add("de", "7a63506514d66af1ebcf073af6ab9f8b72a0678fea6be24b255eb656fcf633884d37ea13d762c9a16895e3e916956c878f85b8c6ba0a9c975c3b4d1bc3627d15");
            result.Add("dsb", "e08f693571e3ee4cb7dc52fe0e2a393b77d90ee4364f17f374faeae5253614db903d84b1b04a421791ac02c77702ca0e13ec789c28fbe01351734353279537f2");
            result.Add("el", "279437920615dfe91b5ea60cb24b838ce08628a8204a43ebb4be6633b41549cea8106c0da8fd3047f9a62e5cc0fe34cccb5a5ccc668b21371144a99594f1e713");
            result.Add("en-GB", "8786a3328830dbd233d357163f0c699ed9b2e87d17a4b1c38fceea2641c64b492c3b1d85f32d361dfafb62dd0c6a80b05aad908ffcb319ce9049b4267ac9988c");
            result.Add("en-US", "37d5cdf28acc97b629a51765487361d86a3baca372f256bcffe0762eabff21aad3abe708fc6deb2f27775d4cdfd078bf91b97769e744e0e346a2ab0a2a6f76c5");
            result.Add("en-ZA", "be145813df20824a21d3eff124aaffb8351deac818b4ddbd8c5859592d6655d3e377b18cd6ab7bf59b87ef1392fc6adbeedb45b4eb52046ee4d07e4faf94c5d2");
            result.Add("eo", "a00cd850c887ffa47d8d5773dabdfd0c433669a12b5eefded33171077be90e91ab67b33c96a7d64b00fc1a2d1e14d9260c83a98fde13646ff23e84dffa35c786");
            result.Add("es-AR", "22eb5732c407137f4a4c3a4fa5305eea91bab0f6814e75e68474e3e5b830342b21bed76c849259e063012ca47b8df35f2628cd3f3cb605dee0ae6e4f5a05955e");
            result.Add("es-CL", "87cc0f3ebd5ff4ffc610299b38638167e5fb958e824b981d7612ceeebcd52a69db239233c0c731884b1844e498db77e359acb5a5865c3752fc3c7078cef1eccb");
            result.Add("es-ES", "4196438040587520e1d3079f95ebdb660fb45609f2d411c7185916833c6772bee5449637984e245cfb016b52ccc026f9f51fdd06297bbc2e89e07035beaf86b0");
            result.Add("es-MX", "29a13b0f5c8c0f0d5023871586ca62e576383643954eefc00c03e8302708fa9100f2a89959cb71a2bb15bd9a24178df30e1d39cfeff6b6793be46a0cdc2db193");
            result.Add("et", "d2cd1fa29d330746ea2eb32b88f8e8db95bedcd75df03c39b1af44719315967e9aaca97c4bc24c8dcefb960d6918e736797f82826d648fdcad851ba08dabc7cf");
            result.Add("eu", "05f27f8fafde4caad93528321cd9164adac11babc23d79b02d78c27317c5c1c6279d6ab5301dc6226cce1a1545a0645197caa42da74ec06fde8be137e9bebfb2");
            result.Add("fa", "c09cbb36636fb8bacbb2858a6a4646516738c14b113445adaa6fcfc298fa15f360a2875b4ff9c522494eda9b3d89293c06f7b792b0f4761ec38c18091f22b216");
            result.Add("ff", "4e72480d8e26d9580545b250a974f0c290d3efd32509008f3f4632dab3b35d30796bc0c6dfa538d7be4a45f13cdd7fbf61be4f49c2877ce8f79a60714ce0bdfe");
            result.Add("fi", "07e1d6ab5353ea3f4250d292d2424db5d750e1743055802389981daf20add5849172a19fca3e64f904d49bf39dc571f1a78fbccf8f67a68ed995815779b4a91e");
            result.Add("fr", "237fa0dcd3d275c77fe319207df289c0c601afe5806eb59b541826b3eaf8a4aff92528f49b5c62a9f09699bab54499efa296eabb63aa0a0936077ad2e953bd85");
            result.Add("fy-NL", "9a4b63cec3b0b5cad969b403614dc0e3eff1649b75be7f6be7f66696aa4039e4bdbcf5dc265070143c90186adada045f3572957efaa665bff869e6b183cb5f1c");
            result.Add("ga-IE", "6a398315012c0141f09a665171f1882a0f88a18d7d7e8bd931f67fb5e4e377a86b6ab990f2351873dc8258e53ea9cd10e19eea72b758d2afcaa056f82cd9bf4c");
            result.Add("gd", "a7eb04b168ee1fac57b87e056ac23b7f148d53e17685af34fcf62c44436e4ec037b583466ffd498d14b12072996ed43a01bd36715bdb5a86ebef3212d9e55294");
            result.Add("gl", "29466c867445ed786f2a1e33312d3998aaaf6575abb3d6a6efee454ffe740dd5a7ffb0ab12a8e48433f3547a932c9401c8e4ce8c7c270befc08183881d8af2df");
            result.Add("gn", "f3d230fa2f17666bfea319acfbbdce75bbb6e929f9f719fbc132eee9364ac8c94da2a2e631ecf2137bc45b55d8721c69a6968bc4743f5ddef7e10649645d24dc");
            result.Add("gu-IN", "845a0563682fc5ca8b6cc3f32f22db821e001998a67133597a138a8dd266d6f31d2cbedc7bbe9611b2e0da7864545a5e0237493542fd5fad9c8a1e3b3297159f");
            result.Add("he", "778a4bbb0afef6b5d7e5b094a5fbfafe0852940f8414d917203a31f4e014d2a1d0cd8dcb23e24f9fc9f85daa81ff7df5989525e2ddd36473c4672d9de9204e9a");
            result.Add("hi-IN", "7f540fb856cb0bf8277762050cf039cc92f2096e74b9093850c1d91ccf92c8053448c16e515f95e7e6940b86f2f4e7d99b0c9cdd7d0aca76c5e59c6a7ba5216d");
            result.Add("hr", "9ad17b329e00c4c363d32d7e72570620d9f68ebb700c60e5eb1ff820c9a695baf866c309184bd607626a705fb4529a965e82f245b6b008a418ceebb3700c9838");
            result.Add("hsb", "d0d2d64b00d14a764d79dfea553eac35971b3c7ec386ea51af3ad01926013115b7363acc8c0043d240a21669a0b8870971942a4e0dd60703818edae954ab1023");
            result.Add("hu", "431ca5a434346b3daa740337362005c7ff3ae59129c9664c346e863ad51352a1f54cd35d16e87c769d473a3690061fb2852b85017f378f0c19905b49f3632e52");
            result.Add("hy-AM", "9d39c29e19f670ac9bebe97e52f5e6dddbe6f01323c2b3dca61421840ee7e35974932a7e3a2e8665e484328c2f06c09183860667aeb899bfda47de3e157bf613");
            result.Add("id", "58994fd9fd9a852ae35d2b28b0413e6e8077c7531ddb5381e6be42adcd55b1fa5622127f0362bfb25be6f0a930e05591abd5fbcfd15c52422728bc7801ce6a2a");
            result.Add("is", "d620a324fded460f6f050e1b0536ad80cdd03cbfc63f99b912f930f6d47e75d9637169c6f74c358b742f0b96918d21a760b46c0eae2e9a858aec3c7cfa8612a7");
            result.Add("it", "5a8dfffcf0bf90c05d670f24052fc929e1ad2b2adfaf8d7baa77cda5d11e46f14e3f9b40c9b6877777b27d031e617af9a5cc01f20128107003943cf954fa6a46");
            result.Add("ja", "fff8b3fb2417899e41232293d7ee8ba03b42ef254bad22ff17b1b994263057c6559bfa8e4468c20edf6eabd67771497698d06cf660841239872492d3b58c5b85");
            result.Add("ka", "027b9ae9023b113677d1ce0c40472f35c0f0c24e062c12c3b35893d40c3ca0d79cedb2cf996e5d9ef0e92bf71d61ea2891fb7f5107e9621e931ca863fdda2a60");
            result.Add("kab", "c658d82ae3c29e22b579279f9f00a68eb9550c50c4220a59a3bc32e62f69c3a4b780e95c8cfa15119a3d8d3a9f6dff65edbeaf3bc2770158d801b1ba250dc5d3");
            result.Add("kk", "45a7a3fb6ba25413e503bc5fdf168f4d8ba64bb423964e48e057ede1d2751294c4689b4732fae99e83d38145526b7023afea9893ab413e0e7e1724b1c026749b");
            result.Add("km", "710d7c3e6113cb8f1f8d0e94616a1508bf4475361b5f99c00b3841b8c4b192204209efd686cc4633faeedd1f764c89ddd958568ae56f3361fec42384568792c0");
            result.Add("kn", "3c705cd0aa5a1a6ee85e98b7e11047e781e402bd1c9064ecedbe977109e1926e8e3a6a13f623a3dccc371024373ab9c601a82069e6ab4310bf29da504da057f6");
            result.Add("ko", "308df76b8b10d887caced54e7f9375fc03d8150357e9d5b189a220c4ac9904872dcb3f977670922a49823c8f5a675aa7270341a700167558d7045b01fe61e779");
            result.Add("lij", "2c2d439b975fd6ce21a90876ed3af3a3190f45677ffc3c10dc77a26254c8556c663154e937aa83399077f1483adc6ceb9f1b7210fff865ecb8b471babae89967");
            result.Add("lt", "63f1278fcafe0fef3e67f5b18bfe30ddaef386bb733545de419330be9c50192bc89cbabbf0a60f49a02bd31248f381796bb28bd57ec828530ce11652efc7925d");
            result.Add("lv", "6793f29fe187984f779002549c23d7f265d1fbcf2614e8879cddc7cf4dcf710f9421967997a00952389f9f72c491717278719f7ab94be575f7bff93e934df3c1");
            result.Add("mai", "16f7b9d76dbadde93b0cb96b07fd0b627d64beda55ba8cfb9ec1d67cdbf7633e58559e614e9e11ee0f093e084123cf5fe207cf216359db87c0c839f637ac2204");
            result.Add("mk", "7d3df19c3aef838ba5c634914bd63e4348a852ed603e4f6a5f770e52e950c6f4cd61e709b135b92a6049a9843df44331f9714f1449f184dc151ae121ec054b2e");
            result.Add("ml", "b6e020d78c6a46a6ff413c7d5b29b8cd3f8a478aa7e18e23997e35a21eb233454553b0113528ca99f03c9fb8ddf279799bcf7e6596f32eea63fbb0446bac8f47");
            result.Add("mr", "af8005f1df6ebd65517a9255e8a773ebc3a9b886f47a104f4b9bbaad5cfa3811420115d1009207ec27277bf589ae1d28e76e16d885b2f26d62f8b806078f21e6");
            result.Add("ms", "0e9f8675f735befdb4d83d4068b675f7eac853551742ee0083f195f7205eb6855d2412223aba945980792567d10caa9f566d168ccc3dfab0fe7115542d3e7eb1");
            result.Add("my", "da2c5b3d2be52f606bee0f9ad52b75ec414b4c1bfb26c33581a647895779c9d13aed194cb2ebba1fc7868831d6a9bfb5c07af47409778780eb01823afc218502");
            result.Add("nb-NO", "6636eaf9626489f23d9729571404450445fda8f25236480b0ca4780858d071d08e85e45ea19d0fecd84bda6e91f88f3a1ca9ec43987d85d27bcb19780b996441");
            result.Add("nl", "a76247d845af0ae8798a8cdab9699b9c45f22b8c65b54cb2730b4cd7515afc384a233386a938f6b7ecd78b5307147a14bd4c34bdc08d3a249cf7c231f44829cf");
            result.Add("nn-NO", "652035198a53e9075622412c456ddc74d7a341bba0cc282c7dd90f691a75c01daf5ddb0e90bd008be6014714dff4c0792349c20f8e64fe73cbe86cc7bf035003");
            result.Add("or", "1043b689628a390d318fd0043506c2c1b970d927c4438e75d92947a551970eeb9c52777daee0824d6d161c5689d6b148c81ffb54e479e7ef2b91f8c06bff8e84");
            result.Add("pa-IN", "86ffcc906814c546a6d52235762da5ec9cdb15153a05d5b02df942fbf11d1425345dc4edda89749dca12e6b7f06b355b1cca6caaf780878a8d168442ab140a57");
            result.Add("pl", "fdaeaa7a72bf9ef2f12767e324dbe49b26d71ce7bd23fbb07af9858970c0d071b915b7449abc26b77612e78da2d4722f9e47c55447c5aabba6c9c4722f07ce85");
            result.Add("pt-BR", "6c1683a9f88403e4652d5d6f8ca5680e08d91fd4a675c04a0647bfcd7a9265cd9c5e12d47813f7b8a1a1a0408d959a7ba6aa6e375a66976f1bc103bcbc53a85b");
            result.Add("pt-PT", "939261bea2daceadd3731c790a4e3470d22f8131e90ee2372fccffcce6e9f23a1fd0528ef52ee2623adb7f6949a66bbd57429068bb5da94c3c8b221f5aa8e6dd");
            result.Add("rm", "6440d8fa69bd32c090dbf67e07d98b6285b6d8945d3b1f1ae87a2dc8f42b40ff166860259318157d5b0a88bb17f0f55eae9371e0a33efd891d2fe4b66fcb6abf");
            result.Add("ro", "c87c740a9188a16b2297b888e0bc41ae1355ac80bebc603511d10ca076aa533822c466ca9baa30f293c1dee9dff0b8fede91454b63b1d6f84a80252c21e04e32");
            result.Add("ru", "bfd3e8697c52c296e376d0655202e8ea92564353063b76008ad4287357454577e3a52854eefe1eb17cf81e4ec147f5f42a848d280c5c97650f300bafcc18cb39");
            result.Add("si", "ad4bdc0c593ad8f051d80acd623327328e07b5f97db94df7713ef1a6157e263bfc84d51d5b52b9f10586329e67c8bfef26a3f63d3c207926f1548777df7484b3");
            result.Add("sk", "82688fb86565bc32e7d5361b6e74fcb371634cf6af83c0dfdd3ac0f1f6c1c18e1aca0f11bc61dd752c7d76dad6c7f6559f65efb4564716d8d6116d5068f5a6e8");
            result.Add("sl", "b6890df8fbde2955634b721a04938b53ac4e9a6f75818c76f3e78bae36d4490f6cec53ef443633aca2a7b5c351d118005d21a7abcb9431382b09244f09757a7b");
            result.Add("son", "92c49652ddeefb53b93b53f2961e97320beac4eab7a6f9817de6cd68b341d77a211d7a035f39d4fb29d69f04bf8e80e85662f582423815acc9271c75a13b115d");
            result.Add("sq", "51ae76b53390007b8e6230259cb66e16bbd5496c0d6e29eacc22a6eb80a0bac6a8069bf363614431a6210579e49ef36f2eb9355c20d2a09062002fc848891fce");
            result.Add("sr", "f8181a06ee8880f0668ba3d2e4f758b8f4dca1648cb172d29f542fa69fb3ae6a9e7768b9a24128bd9adfc1260d36b726b5436e7f8a63c95b42cb2d901caee660");
            result.Add("sv-SE", "18d4b223c1d2bc1e578acc2001f14ee51885be8625cf5b2b727adb3d9ed0587b2cf863f53adc4af948be52aea0a70890515b9f1fbafac5322c431efd1741ebb7");
            result.Add("ta", "5b666157debb5dc1bb20bb9def8d7165dd3e0825feb8d885f53974d5f7d0ca57e2b42fac8c2cbb120437173f5e61df800bd0b58187f6bb67ddf00d2bcbc45fe4");
            result.Add("te", "56acf0fb1c8043295f406bca2a00b43804ef9b128cfa643d3a0068e223cf18e3288a1f5cf9f3002361b427ae66f100f61b66753139ff99481e617eefab59845b");
            result.Add("th", "a47fdc162a4dbfe5d9631bd8147548c8db39d47fe1cdb75019773c75ee8af6ca0848447b38d8bac981621e7c0a328303a122063a371f75ba589b4347c871d83f");
            result.Add("tr", "a0a93414bd534e1efbf94d8e11e7121eca21b2840fbc288d9007421c42d38521a29dd1129b4b9388004bf49bf131f37fe5ae3ed5031fa963d0ac498f7253dd40");
            result.Add("uk", "4533ad2351e59e7584f1945bb6936a57c159ef28a0088d4cc55b3351eec3ff690157c7c99c70644683df19a9c3d917dfdd8f795ba7f5648697273f83a2b2f748");
            result.Add("ur", "bed140de36f82554738c01ac2b3029c3258ea8f85c7797459dee29625af621cf28990c376d1313789d553c26fb305936a3a2248832045ecdb70ca9816406788d");
            result.Add("uz", "f349f75ce1b9c6fcb56a97d108d9f210e0d4e850181fdd97ba1c75ce7f949a0d6dec627d8d91604c456653ef3c91c2c392763ef82acd1b0375a50cd24ad4af2d");
            result.Add("vi", "10cca4641b7009f775946d33415ce8d4fab87958c9ab8325481eed7c886b8e2d5a00a6519468bca05c721a2684125ccd0340e7414257e0eed3174caed210322b");
            result.Add("xh", "f0ad1735371a5f6ec9d5c044d824801b43d4e68f1f9fc8e1c070de631be38f11814b60de319fd5ff8135860092afa899e0f2806404ed060b834432909bc09665");
            result.Add("zh-CN", "52d422d50730ae51d4776d5207f3fc0642d614743677a0be428484b45e474f63f071d97d570a282480e1c8c3a0a505f5ccfae3bbc2416a030addb88a46efc9b5");
            result.Add("zh-TW", "8d998cf88775e61348224df5ae72088722ec161813c1b9f455452582c34ad23eeed387648df04ee40324604cb08b92339bb554ccf944e6d841e089fac6a94776");

            return result;
        }


        /// <summary>
        /// gets an enumerable collection of valid language codes
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            var d = knownChecksums32Bit();
            return d.Keys;
        }


        /// <summary>
        /// gets the currently known information about the software
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            const string knownVersion = "55.0";
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox [0-9]{2}\\.[0-9](\\.[0-9])? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox [0-9]{2}\\.[0-9](\\.[0-9])? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                //32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    null,
                    "-ms -ma",
                    "C:\\Program Files\\Mozilla Firefox",
                    "C:\\Program Files (x86)\\Mozilla Firefox"),
                //64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    null,
                    "-ms -ma",
                    "C:\\Program Files\\Mozilla Firefox",
                    "C:\\Program Files (x86)\\Mozilla Firefox")
                    );
        }


        /// <summary>
        /// list of IDs to identify the software
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "firefox", "firefox-" + languageCode.ToLower() };
        }


        /// <summary>
        /// tries to find the newest version number of Firefox
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=firefox-latest&os=win&lang=" + languageCode;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Head;
            request.AllowAutoRedirect = false;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers[HttpResponseHeader.Location];
                request = null;
                response = null;
                Regex reVersion = new Regex("[0-9]{2}\\.[0-9](\\.[0-9])?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;

                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox version: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// tries to get the checksums of the newer version
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32 bit an 64 bit (in that order), if successfull.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/firefox/releases/51.0.1/SHA512SUMS
             * Common lines look like
             * "02324d3a...9e53  win64/en-GB/Firefox Setup 51.0.1.exe"
             */

            string url = "https://ftp.mozilla.org/pub/firefox/releases/" + newerVersion + "/SHA512SUMS";
            string sha512SumsContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    sha512SumsContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer version of Firefox: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } //using
            //look for line with the correct language code and version for 32 bit
            Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            //look for line with the correct language code and version for 64 bit
            Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return new string[] { matchChecksum32Bit.Value.Substring(0, 128), matchChecksum64Bit.Value.Substring(0, 128) };
        }


        /// <summary>
        /// whether or not the method searchForNewer() is implemented
        /// </summary>
        /// <returns>Returns true, if searchForNewer() is implemented for that
        /// class. Returns false, if not. Calling searchForNewer() may throw an
        /// exception in the later case.</returns>
        public override bool implementsSearchForNewer()
        {
            return true;
        }


        /// <summary>
        /// looks for newer versions of the software than the currently known version
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the information
        /// that was retrieved from the net.</returns>
        public override AvailableSoftware searchForNewer()
        {
            logger.Debug("Searcing for newer version of Firefox...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            //If versions match, we can return the current information.
            var currentInfo = knownInfo();
            if (newerVersion == currentInfo.newestVersion)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if ((null == newerChecksums) || (newerChecksums.Length != 2)
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
                // failure occurred
                return null;
            //replace all stuff
            string oldVersion = currentInfo.newestVersion;
            currentInfo.newestVersion = newerVersion;
            currentInfo.install32Bit.downloadUrl = currentInfo.install32Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install32Bit.checksum = newerChecksums[0];
            currentInfo.install64Bit.downloadUrl = currentInfo.install64Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install64Bit.checksum = newerChecksums[1];
            return currentInfo;
        }


        /// <summary>
        /// lists names of processes that might block an update, e.g. because
        /// the application cannot be update while it is running
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a list of process names that block the upgrade.</returns>
        public override List<string> blockerProcesses(DetectedSoftware detected)
        {
            return new List<string>();
        }


        /// <summary>
        /// language code for the Firefox ESR version
        /// </summary>
        private string languageCode;


        /// <summary>
        /// checksum for the 32 bit installer
        /// </summary>
        private string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private string checksum64Bit;
    } //class
} //namespace
