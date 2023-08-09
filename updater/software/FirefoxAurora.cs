﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022, 2023  Dirk Stolle

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
using System.Linq;
using System.Text.RegularExpressions;
using updater.data;
using updater.versions;

namespace updater.software
{
    /// <summary>
    /// Firefox Developer Edition (i.e. aurora channel)
    /// </summary>
    public class FirefoxAurora : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for FirefoxAurora class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(FirefoxAurora).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "117.0b5";

        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox Developer Edition software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public FirefoxAurora(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = knownChecksums32Bit()[langCode];
            checksum64Bit = knownChecksums64Bit()[langCode];
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/117.0b5/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "8d5409d002957ea6b2e079e46ba0b236e3309dcff6bdac9191d5e3c1031aef7aed357a2b27e7033a872b3675aaf84819072e8700d9705b903f1dc65be3c18a50" },
                { "af", "6fcfbe4b43ef308cf8d5c9bb18b6037c730b06a1f864f3b3fbc65175ae22de798fc151c1ae8f8dc6c5a5b8e1260367d38b5f08b0a4fc401b37d810f0c767bdee" },
                { "an", "4cb1a16dfbe7ed11a2387ecb72ac734c317b581f299e8435b91b5bff1d288d1dfa00718d6eb62a0e2b126a760d7ee258cb64658e783353c2828812c68ffe42ba" },
                { "ar", "7d230f7c12a75a22f3029838b86b032608c696702c899d7e8ceaa0c6954bf1f0cfc068b782d8080bbc5252dfe7d0b3c1aaeb3ce4df27880b6252e290ad96487a" },
                { "ast", "7df26f9fffefa922869d37a68074fada37b2c0c361a887f6112730ad357824a94b79d242b5a3bdffb12ab09564099e814c6c2746eee7d9c01f00e08f4f808799" },
                { "az", "acb2e977a8aa197829fada4628511698fb29ea29d30e6ecbd92e4fd820d6625aedc213ce7803d7e84bbe3fcb22b27b33aa3ddedde812733aa5d16fd54a7ed9ab" },
                { "be", "f81ef4c669bfbead320be4020663a3266bef636fd7b2edd156a2082c79a5a0d24a68b8caad49239d27653a05dfae6c4989a0573801b487c80056b9291507bf00" },
                { "bg", "f9aa1fa3905f88528b4158975d4b0c398f080b8a78f3d3d885a7442b91605b0e3a8818b3b215e09b73d8ac6901de6c23edfec1833178e962989b417b55e314c1" },
                { "bn", "e3a96c075de6a580771969d27af05a44207245b111b2b68fa3ec4c82165afa30e0414650dac3adbde608e83c4e423d38ed9f52bbef66bdc907c1074b153ad55a" },
                { "br", "1fa88d91d5ca997fe9851b164df195de58c0c954f2b0950c33039c54c071b95e82e49f0ebd17dd868b10691384afa92d2b0bc165ee908003ff0eea458ac2d346" },
                { "bs", "81d06efa854b6532d0791a4a64869a809aa4b758438838873a8b358e1dcce2145c32a1b9482192bf174c6515b74eb7bb568d4e38c359b5b0eb823988e4255d84" },
                { "ca", "b09b93373eae62bcb2d3766eee837e735135c3551884bd4ec1bf871ffd1c8dc0275815f16f9c472e9d454bb79425ee6dd2137d08ced2b706d6612b30828a69dd" },
                { "cak", "b1cb7ead63180133e6b7fb210de652b091c860389a28cabcfa6737106867d98f89452921f3fe807540311bb2213d0488330f19b054ce79599baae149e8b9ec46" },
                { "cs", "92bf7d0997dedccb4e22fa44efb877ed026ecdcd83012eb80e4afd154fae03f5500a6a53e229f6c7ca7ec9da3e2a5f5c31d2090d69633c236d65d0f6270e6233" },
                { "cy", "df10e41853e8683f37c6257e823c64aebd9581eb67783974ea7fb01ba3cfde8ca96fb124e282162778045630cfa142820e0b4f28f0f85e45e6c1441f9ddf0312" },
                { "da", "f826143c9d0ed8256847e07642f80ae7d679972a7d25dcf410b1599dbb684411edff24959630a3e47968fd114570a35ef67b8d56acd9da655fd2ef2093b3ba25" },
                { "de", "f21a858766c7006f40db579b8d6d138916877ab702c2c7da7537fb4b3cac4386e532dd35713558d8d5a217eb1859bce77d83aa7215afeae77f3c78fe90ad42a9" },
                { "dsb", "5bed096b54f239109b42ee0562a7d2a18e69a7e8ffa8998d93d0446b503ad6d048f35b0c51be344a2035a2696ef544214a0ca20baf6929211952bd1fb070b9b0" },
                { "el", "4e4a98ef59ccbc05c22990db9da86f642e1bc539d7288bd17811fecc4461283c9b56a6127450f1275ad4cd68512b792277593f01e00d3f60d52dbd9d722f7136" },
                { "en-CA", "7e7f6ace0793afc075c0204c97141e9ed53df25581251860d9f9f6aeb172ef0f4e0319ee657951ffd119c11e79b06174104b22e6243ba742edb2b7d467ba8f81" },
                { "en-GB", "39968a1bea197aa44effa9c83a8dcb7f0201d93bec7386916548c2491432738775722bed623d7ed90f502e6071f21952b238d0925a581a5d46d862906ba6a5d8" },
                { "en-US", "28ff3b75246796a8ce708f58b82c3b92721f739fcf03e2a1ec071ef6f8bd77ef4c45d9e37372ed6898620f3109de380ab2cbdce7e395b359dc18ffd855ae9996" },
                { "eo", "c453d5b5537bf14ae59d31024f2f1e93c30f04be198328edf9674ba9f044860bbb8a70fae937f6b2ff3cd416de627509fcd8687661d2122a7f9e493853fb42f2" },
                { "es-AR", "944b04aca92f7563f7322bae505fb677966ccb27b9cce6a446385afcf9242176d36840e50822e1f745353ba8270834b90356606ee828f0859d9c4afd8dc45fa8" },
                { "es-CL", "c8774c9f9a40d58de35dcca1640d76ed86b984f6950e020db7863a3b89fdc7e2a229ab8e584955c1bf91d660b5281a69e278cce7717ace290f0e10223f822220" },
                { "es-ES", "0665db93dea394aa65b28f2ace9df286d364b1311af2550881791c85964d7403e0b957ee4242559f94f71da9689cd254c78da619de022fcad89254b64a4f98c4" },
                { "es-MX", "49ce6b118226dd6cf4471338dca4f45754e01c5ae277578bad381b69fd568c54aa4532599f68ae7ded51ca70c483fceddf57008f7b8d99a774be80a59a35758b" },
                { "et", "357ebf84662d8e27c19752e2822d344c65ac9bca6fe6b6347dbe57340caf6dfa4e7f9ad5e0068c4627a3b97d18a60d535cb983c1b09237c5699432da8e920dc1" },
                { "eu", "ffde802bcb4f296ac009e8ef115f35fe4ce96a087a9b621d2f411f504a478a7bfefdbf31a5e64bea6a2a345e507c4010c2145dd8acaf472338c9076e68b01049" },
                { "fa", "bc3d6295354cde7f69e46ae4d5c208eb16068cae3d4bc00d8a3748e0fc816e85faab5f7c9e4c698632f485e5e50028a0a7b4a5ac4ea180bd749fcd1a434a4ec5" },
                { "ff", "76f204ed7cb8186f48ce53166a0af1e06e26729a116b405dac24c2a18f41bda000d96092f67ffa8f9442f83e95d5548031199b94ee34a57136db29bbfaf5666b" },
                { "fi", "1ae8128e031f5873075b5d11ab2d8ea69c8e26a27f8af9ea119370cf7ba8893e70034bfbd8ea588591ba86f557508fc8682add5a64e5a16d6c1dff3df5581ac5" },
                { "fr", "b0683d6eef6b03a1690f237533dcb575ee6d8e48ef926d445c336c848f6f9296a1484e6289cd849b4781116dcd5839d6ed1690d8752b698c298d9ccb10ebf8e5" },
                { "fur", "0ba7efc31fab8b9da01285d28ef33fe7622828884ea51443f4addb480040413cb2bd7e527e79f98ba0416150ba66628127936cf4e371325cd90b0921d8f61a3a" },
                { "fy-NL", "4a366155e269b17b20ce14a7caeb60bee1f523c47e4a669a74b259c865a3ae1fb281820ae4e1882a7fd6957e8509316d090de1fd3de33e0e6dc5d4a048829763" },
                { "ga-IE", "e8e39785e3d7b68b92937a0ede97ec2f2bb484c504bb873e3549bcf10ceea7790c0e91c30d1f15ebd83b497dd01de04f266b019a7158ff68b331e176852fd192" },
                { "gd", "ce1920983b10c57b6a652fb90a7885e2f19e4313eb8a7388d10810ab94b3d60b26363037ae4791bc780bad2cf380ea7924b29ac558d1ece7333eada2a009ad40" },
                { "gl", "f55289251796bf06b6dfca421c094fcdfed3efefa888dc86d63acec73c05f2a4d2f96333ea56e73704a8660026e70047b019fd61a83a8563ea4345de458021df" },
                { "gn", "85fff32268565e3900edacae0973cd97565e727b9e02747fb7e179b52edc699eff28f4482e234c242c82e22799a0c114e5d533312488d7f11990079b5e5a08a0" },
                { "gu-IN", "f47664e7442eab5c97000d298ec33411994915393c82f6fd103c8e8e92fcb1a7140ae061ebeec3d35e15102d3c7934b0e54da1d227e86fb362164b33f8d76b1e" },
                { "he", "7ca42a00b01cce276ff250f7f43535bc7cfbd5df866020280219245a4026342a4a38cc8e2cecab9d2730dafbb12c98df0b852d3596082ed3b4beac396023faa7" },
                { "hi-IN", "b69d35039e9f22f9bb4cd11e2259f4251ba87c63e560884b60f3fb6e97bc9f4657da6d3561923331f9bab5a14b7aef16d6d4377d5cb3ab48f1678a4bcc2b9d69" },
                { "hr", "699985df923a799c2edc4d9868549891c08527edc7e45067ad9e859adecfc93224da002dd62a80d5c17bd1267f1aa1683f92d0804189095742f3946a4e16e5ee" },
                { "hsb", "a4444a503bd4373af60c26529abeecd0d4872abb17b5d4ebf7650d021fe3420da6eae9b1a7526c5cf1374b6135619aab2d60abc9038255d31815c121537baaf9" },
                { "hu", "f99ef60128ba0527f711448cc2d9f174270aa658b5cdb30b3f18f734c8318d1192b3af37b22f95f1df57d202977a484ded662e0b5e1cb9f495e0880770754369" },
                { "hy-AM", "a68bdc857afbd52122c43ddcf2cea9f480d484187077ea40636af50ebba13b10e11021927b63bdd2910274a58b6ad2ed6b6acdfc566b57f59c1b18609980f109" },
                { "ia", "31315556faeae8b29ea69c7b5acbc27a5f02ae9e6c937465a60aaf6d226040ccbb747b23e2fafee53b1fe78bca2945f4feba0a9cc2359a7864c227e0b9f2b013" },
                { "id", "84ecc9f679160f32b98469f4832dd2b397a10b93f3d82e6c39639bf47ff94215632767a2b6f5550790467864f707bf50f1dae7a3f82e7292653deeb52af57205" },
                { "is", "b542fb9d9346e84bdb681b16ce3c0704d47bcb38803d68f4eae79987427b1096fd96374a7f46b9536e2122a101e28feef7198c31a72954526d3a4ddb3e97142d" },
                { "it", "177c2dfdcafea433268cd67af34512920e03b327ac1ea440d39df61068ebf36b6f30aa1a4e541214a10be82bf128393aaf4ae6bde43b2b8f5819323c8164f979" },
                { "ja", "11666844d9bee126f0c59c62ff255a4aeb92e288b38c07e81b3990373111c0a49f86793a3d39f8eacad2597bc8369dd86a586b3c505ad6ece5b0c68d01f19482" },
                { "ka", "d0afc6b73946a3b17fb8ebd1ff8127c9ebb462a51848df3432b7607c6f05c8d21ddadc0e66236f5882f73ab0dffb1a5dd8dcc62983d552be2df2c8aed71c1e26" },
                { "kab", "a2a6872f4af17a75ac827416763c2e1e0a8190697c6b9680f26ac9a7b41c606eb2e1425e7318afca9e4e26e57ac5ff2ec8117920f90df8c96f44a8869a912979" },
                { "kk", "143089ccbd2c736ed8269cf1a800c9498d8f4579c8298ea08078c8adca169c3a3d47ccc9c6c49219ef0dad36441726820e511a3a98002204e9e8578f9341ed46" },
                { "km", "5599cf0f42b2ab74dc3f369d9e7484d9f73195d4a75b6563716a8f1581213d4be64245e7f869d313daad97c16f3ab67d45944c28d506c6d9448f592675672d7b" },
                { "kn", "00fb1a7cc430ad10f6b0ea5db8a85bc395103a0434c61f8f986941056958a348c4dabc8a1949f56b2d8714e8142f7f313e50b553170be2ecc23422352bbc795e" },
                { "ko", "30f3182b992695be72d724a3ec36d34f8beb7679bc29039ed75f1014e7f3c4f134f973a2837136f24e32bfd4c3bcfb8596c57bf760728fcb6807a6a874ef77c4" },
                { "lij", "5403aa71a32b5aa9e96cbe1146e97befb557716c77dd4fa8623043e073d90f40a625562baa5391ebc75edeca8d5d7efc80549e306715f2c5e9c08787a1c2235d" },
                { "lt", "ae5c2330c0d6b665a5a7bbe2216c86f9cfef25234dabfb95bfd39c58eb210327533ffc8411c3f285a4646f704be0cbee4686989ce08f0a105ac0ae5173451cce" },
                { "lv", "78b64b66a2f45feb017baffd0fe84d252f13b1c0ce7e7fb83cb5e2fabe7cc845e29195bf0d1404dd28380318d57e7bc27b60c23b323cdb32767fdf369b75a150" },
                { "mk", "5b4a325e560c12b65c98f626628754743d7c243d4de69aece4f6b5ddcd8c4bf24655d0c346a61574b5530c65af70c580b9b64aeb100a64b0d7b35d3779173592" },
                { "mr", "6154c454d7f9a1c432be5cde3157405188bf252d020f300cba14607f638fab3dc1d493f34acf5e13e7b3c9db01c15cdee46c3676d976cbb990e5b582a7f46a1d" },
                { "ms", "851be8b2db03ff5a0a2bb3c86145f507f8420d34bbe51a7273354cbe9b8adefb2c5ff1fb168083e5b4bd60183fac1892b4f779ff66927a63fbf420f89ce675ba" },
                { "my", "c16b90b9848c17cda4197fd60f4eaf18439b80ca6b88dd800eae72cab6fc2b92b88ecae7145fd9fc055ae726b6d16a0066a1cfe1b88f8b508c330d488323c9ad" },
                { "nb-NO", "675159d70538a248fd8c9a211ec0f590c71277037857591d7a100b623588e2513e5ecd6bf81aa68c3082fd6c70b5b9c22cd344d712fcc85a3a12dbeda8c57bb0" },
                { "ne-NP", "ee56b9bfad08d587cc0ab64429246d3356d86116459dde324adae859564cd6bc702862ed85f74e85d59896048992221ba1cbd83bfc2e11d273aa5c555def9d88" },
                { "nl", "1a251437ff56026b72724f828dbd8a0d6e4457b6ee636c4f18255320fefde71db890f932fc8fe2ca25d25c43ff3d36441179d858c19af727c9fe77af09046c97" },
                { "nn-NO", "24d06ca57eaa363d3b29a639204e487e063feaadae716dae7493aa43323620bb0f5dfc95e3ab201a8dc309a28be649c77d5a255bba4cbda987c2cf7a516f5517" },
                { "oc", "0c2dd7886ad36862058eef9ff7c5330f42391772e2b29ff4db4436e9c6e671f864924956d0ddfead3727cbad2e532f5ee8f4afa0fdd73119a0a8bbf68f412e51" },
                { "pa-IN", "1400d7d16918f05424206f1548f627f8d268ea3939c2035e267b71ac6ba93767b1b23136bfbc2263ef71fa5b8429f9a40f1fad8b73eb150a3ecae9d3798ac151" },
                { "pl", "4177a9bd720157a8e859e69aa479bb7c0dda7d71d340de40e70c541e5005b7469ccdcc78f3b2786582639d59c5d1d85e2bb7c5e3d02554d3a4f4abfb119229df" },
                { "pt-BR", "45059410a6150e5da22ab53a25a67a151e4dc5f791ede26813c9747f1dde53fe104eddcb5679a0a23356d288a7b3130748a6dee8f620655ac6d238e1c38c17c7" },
                { "pt-PT", "ab21ce3a4f054c95f7949a03a17b8aa6ca446629a84a6a64ddb62351b0a19f49f63e8295071dee083a589f7b1a73715e45b5a5aa1a608203d76c9d4033b4d0d0" },
                { "rm", "f67b770071411a9ddfe093926013a8f0dd7068e323ae608274c06b5aae94490ece85492b423e7e78b2632b73cf8e339aa2d25c5674a11b90d808d8fe7ed02c27" },
                { "ro", "1ed501e151e5b64239a009ad9693355df8541f33e75d8b3793c19b22667ac931deb63b56b66491ca00876f1b0314ae7e4023244e54d7f6cbdc87af65fd499fb8" },
                { "ru", "eb3aab65dde9d7babae86d42ba1188bdcd995d82a62ee20d80c707c1cfb5ee6dd9e29c60212e3697614d2c5cb2600d37ac59b30b27e0175a75f7d0d289b0fe9e" },
                { "sc", "7b1e6ebb1439265cf450115819cbf8a14d13fbc44a6839dd5f85e51d4b614560377190b8dd31d368b4f7f3e89ecad1c76f290ecb3af06a266e0e37941b809949" },
                { "sco", "3e58fa88e08bb3b7469c0c73669c7e82b340e869b68e1fa90ce7bdadbb6a07a043228839556b11640776790a499926dffc54ae14e973a7abe3d1b7dc98c04c26" },
                { "si", "3e2eeec5897f2e3703446be27817ff859c0f16c81817debada4725449ead1605378e2af004c98b1bf09cad28cb0a4ff01f4e7dbcdeb62e82d9fe4c93dfd91340" },
                { "sk", "2ba94bea7611b12a13256ba5e8d828fcec7fd175e1d44ccc006d73772444433f64bc786b974d0228fce4795ae4280c4aced19c1e5e54776fe297037a2e08e5cc" },
                { "sl", "59d4681269c940aeaad6d612455d54303b48e1659b2edaf2fc743b2de7a67fc1905dd0258bdc5ae8079de0a9a0aa1182dbadf0c0b0096cb447d90bd64790f201" },
                { "son", "ce46ec41b9b405f00aa1b021bde3ac599a17a294364902dd1a4f88360c6935520b7c11cc242e2dfa48179183936fb3e79fc4384a9966c30ef6cfb2cfb9d60b1e" },
                { "sq", "eb2b10a9fadd631df1bbdaa67b03f072ace7c1d8db1ee0c03709ecf8ab447d595417d5467506b8a8992347f5007e4a4c302ea25c1c1c89b264f0ec17ddfab9fe" },
                { "sr", "891ccc17049c56b4a96bb32d3b7af4fe0a475d30f454c82545130fb9e8f476855cc6c46528c17850adab387284349d13ee2f0ace016c43b239e39a5983658722" },
                { "sv-SE", "819f36c1919cd0adae7a9e4e968fd21dbf0fc366af42858d2d3770b52f55feb92040613de8c9e1c9a7d08682c7b79f6a8a20a8fe2dd991b37049303050fd077d" },
                { "szl", "7b734ad2c815f939bcf98ff62845137bc1e7aecf2ef3a9701a1013ddd5e03470fa475134c57ee01e315fa5faf737fe7fdbe66ca0f1180ad032c012b38fd90dac" },
                { "ta", "883185b7925e23a93d9bc118ed2a60d1df1028401a74629e6a07481d5eaf4a433aa8239dd212a7d3f729d05d37317c89eed85f25036a6f12556f7a9f26f74878" },
                { "te", "c70a7dcaba66626c4cd8f32948b1f92607fb8ec46a14e39fcf5495a2d6429359449281bffc024bbbb0e6044e4e3db2248ff662137823044f692d483cffea15f0" },
                { "tg", "3081d19ea601d6726449cbac8a802ff6e88a4b94b50fb2e6d55dd9ff0926428b63a3d8a328f0c357feef3c8a46c82ff6fcd2304f031912df4c04ed0505c054ef" },
                { "th", "e6032b8283ca9e9dd0a339176f03987a0c2543da02637287c2f5022a04d5b6e2cb0bd3116645bbda8d7bc90ace43ea7eb53de567039f20f4f0dc0ca764ea83e3" },
                { "tl", "2ace928566030c2eeafa6f4d6b61bf85ead52a424a5ee4cdc29ac058e735df2634dc2adda03740bac90d8dfe205e9eea28705bfc47e700efbe3629525bd02b69" },
                { "tr", "38ecca887db42572b452bc76e67a759985bef53e40ebd30f317be00612ee629f2e78c35db244e500451dd71366be92088a20283adb87e9032c711e4c4fdbb43c" },
                { "trs", "8befe28dc2f82cb465283d5a4059ac9f0d4b78a8c72e0c14caf514136dc94e6b63c6a5c02b232eae01f905a9adbabb91e412cc29cf02798a960583dd43d5402e" },
                { "uk", "9f72cd283713eb15209516c9fc006ef4f6ba5e286dcad728d11a397d602a0a6b29450e6bbda290f88d1403b55d5ab18fff735f688eb7ca709bd2f2492a39f9ea" },
                { "ur", "83f4d93def19fe8ff84ddb84f374d7e3c0d5b3711cfa91409d6dd9655876d93816adbe9389883fcf260af2f029163d29f96b4be54710670560939e18a8adb53c" },
                { "uz", "d81301a92e746c900356a97fb43e0e9de1d03e6a94d0d73d1002f92d0a8946aec3cee17b5ec381ef83fe78e0c0f84c65ac704ad0bbb248aac8f35a6ed8406109" },
                { "vi", "53669a711157e18a9af7449fa2630fffed75136cdd65a7cd6bf2f02858ceaa86535201931be434e2c13574fe66dfae09633cd6fb93fdb0796aeb7f1df7c1086e" },
                { "xh", "f39efc6b2f658119e9cdfb14047a66548b0b41bdc9446adcd427bd7979f350afc3b4e3c30a4b713fe7024ed83d68b1d127d7a21139b3202f27a49594556a4363" },
                { "zh-CN", "e7c356fcd1f757d14727ce9af6e01a9b3fc53ea874803ad85c89a69185195a3b6c4e93d9d2b709bfc8a075523e939de733772aba1558f49653222f456fd4c1fb" },
                { "zh-TW", "4e9fe7320b242f63d623b8d7cc8faab877054e50b2f21fdc3f27365992f37330a55720680c9735012c31f3ed798c45e66e4d5d5c7819caf33d7919084bd517e1" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/117.0b5/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "85d0b865198c911530192c903f75331acd0e634af720aed8a1e7e2e3eb7a4a22168d2c2571be2740533727fcc9b126c4243c38b488ef118e60c1265172c9b865" },
                { "af", "5e74d7c4c0c9d49e62cca36411e85c7b8e7ee4c58c608c8ffee0e764b118d7740ee0fefaccafe7ecd2a55486b5ffa5d8316d8d3f719fc401b2b75a308792d756" },
                { "an", "6cf6c53751f501b3baeb0b8b0efc18e616c2cdde8d8fc6847c06e2168a61061b920b8c0cbccba8769fd0d0ad0b8930532c794a699ef2b95a3e4411a6aaf4d6ec" },
                { "ar", "473eb247979694f4230aaef53c407776cff403456850f49e27f968cd5dee8738c11693ffc2ca07a487224c6b7dd7fff0c3e3233413afefa2bed5c56778e7440c" },
                { "ast", "28e8bbefa6260b5cdae63ba8ccd167551ec3994dec345fab714f6fa6650bb39b32d0dc24c7c34a935680454a2d88099a15dc17f1af32c2db09250f46b587e5cf" },
                { "az", "6e8a6312a6b4cabb7869001294f36d84f04a18e585eeaf94ddbc75642e2409821beba9ce6ef170d010d95f1a35c5e6874d898083862aefd9cf2f77d89c60f911" },
                { "be", "1c5718bb3ebb5ffbe820dd7268daaedd49e4a2d7db50448a584b47cf71e6b52b75e9d14050427b0a089b41f1089dd1bdde87e6d27ab604b183697e4c7ee036f0" },
                { "bg", "c49ad3eccda33283295d22138de318a03ca72fe28cadd72bb2c80a201eeec513ca070afe667901f6ad42910493d679b1c9c56ff1459ffba9780c679a64015e7d" },
                { "bn", "b4e88956bc1ca4efb0bd40f0887c65d39113f9938b4f889a8b037e4f735e6632b7ecbf6ec66ef37425c49e8f710f6f0cab78827c9dc5e9ee2dea24f69a5c62d1" },
                { "br", "c6272f7261c35323d333b40f5f50a34c633a7d1cb45b6f4b20dc63525c86f6438466eb363ef4903b86a023b45b93b51587361c3a0befdb4cf054cc606cee845f" },
                { "bs", "fe63c5451331ea2bb7a9b1502de4d9df5b4e3a82b7328d5b186f2af53d0925d04ae57c0b3fd736388160855a0aa457199ff89c72cc2898d97ad701d7e286214f" },
                { "ca", "04a62b4228273b69568acfb05e33ead6218f0681cbe8d24e6de6504979fa8b77b97ec722c53f342a58a9fb6746585ff160b09df4be6a3c0ae27261d06073a420" },
                { "cak", "5f7bce1a981ec6e20d99d06f800a447b176419f5860780ef4463802d602447fe55b7238dc66b7eff4c49e0e43ccb5c68cbb5e69d182a9c278f20288b1d5254b2" },
                { "cs", "a4f2e42492ff3c89e87bd0999bc9e30991d47fdc2da3fd95dba486aefd356ca5198ff94e93b2cfab99feee51a9961d9fb5c53168c4eaa6371db96f3467f1e178" },
                { "cy", "5742acc2a2eb47dab7badf5e08b3ee66719c7d3858f2b36e48c2d93fef177cda74eb2aa32f4d721952163b75b80c9d633860193e826b0a14fdbe69f3e49b5b19" },
                { "da", "5342dd6085699b99ee06b56213307e0bc419c22f1d46095a5b675fc82d7efe64b8861bdadf8f49fa4a5c0c4f93f594ecc4c0bdbf023411cdae22d81fc6b0fdca" },
                { "de", "d1715acdf2bb985c33927849355128b7c7b9d7ad45f4fc05208915ec70e699715e9742d3dd670d01696678a703a6a71dd6773b2be65cf9a71f921082ee0341c3" },
                { "dsb", "f95d02d8967e6c465aa8e0bc050653799840b3fec7d1e985030178ab69603d0c673c93831b880567c63007f7efa5963bfb228298a3fecc8461fdb13f26cbb679" },
                { "el", "a35a598fb12cd5c1ecad4fb07061a7a7fc00364d84e4704d15e30a12635f0d8e628adae868a66d26fb684e0e6702e5033b1c0f8307b7f1c3054e0a818795ae12" },
                { "en-CA", "e6f86b5bcdb2b8aa494e061a974988251079616f2d5741bcba3a6ca15f64cc7005df037764ab3d97c22724e3a115b84cb438571a233269ed96778e6028539d62" },
                { "en-GB", "26912c48923c5d8ad67d4e5af949e9ef52a98ad60d7bcfebcb6cd0c56d066a2e2a59a48a62b7513c8168391ec5a53cd214fe72dae76b8ad1445611cf3e7a4224" },
                { "en-US", "4ddaa41a4acd2966253ddbff3f1659e9eadcd8b2d587855f03f8bc92089cea7b4ba22786a087aec1814414790cb89881b195016989d1564b527427af5a9ad8c8" },
                { "eo", "3ca24bff76315f0089b5a2f0e67b86a2e8e8e2510cc544c3c12e04401f4627abd02137e8a6ee352822e210241af1254701434c5ce6cb02dcfdd529cb4fc36845" },
                { "es-AR", "3ec3b823cc45ec34492da11be917cee8fb7390bdccee268eaaa4306d11a5ef09fbeeb5c496868643059e60168e91eb6d31ab64886a9fece05b549862fe24ef25" },
                { "es-CL", "6f15031ddbcd3d158af030b722a83a9630c7649ff177091059c0386c6289d9684ae66d60342ccc060068a616f5abc9a2786a6e4f8a54d489e18845b705fffba5" },
                { "es-ES", "c87e49a386536384fb11737008659455362b52f8ab932452681f350b2a2e0f17774568af1900fe191417b9b31f0690890b54992a05359c9ab0295c7184f9ed97" },
                { "es-MX", "7245647c63608d3c3cfe482aa5f22ffeed166575409b752e209930cc58fded0fd6edc24821e71fad74cda557b831da7ef13748ea4bb6b1fa1deba94253ad1148" },
                { "et", "8773e76c823516f021ec8d5feb772d184fe9f8a95b7028c896f3275c6899c9034bac4d504557f34df8502fb68f7cf6c7dc7218e88bfc0106ca788fb8b80faa32" },
                { "eu", "c73e78cfa90bffbb407a79c421d356d21dd195527b3469b8d2bfd343f077dd487fe759aeb0987fe2185ead4c9340eb9185aa6e566f8d321844de9b0dcd856b2f" },
                { "fa", "676eafec2af6996502f2cfa478545a4f2c91bf0163777724e03345508d820ab35a7d5d1634d6cafc766ad9d60c02bbce6bc9898acfa65d021f603a6e4b44c697" },
                { "ff", "727411c84e32e69e0aec07d128df2468f68d5ba2f9027c0aef27d6384037407bccf3413e34eb2f04daba1e0a92ece936126202b0b11ed35ff1dd287bbb8a3c95" },
                { "fi", "025d12d01a8795c7422fd6773a1356232b6ec6015d48d15c696dad844cf62728c19082e0c7cb5768db2b6d8cd6b7083faa30e76d9f9946ac2f0afaae61a0f9d2" },
                { "fr", "d32415f5fdb4f763188ca47f329c650c6bb188d92d07fce51b286940ab2217614c1da4c6370c09859d6affbb7b0ee679d621fd0460783d5621e5871768487734" },
                { "fur", "4e9e3883bab61b1e3c2c1db5a677e1be1fef8423781184dca762d1ec6969d77777a71dc8e830a2497da936abdff92de1d4269947336abd9f200edf67550188fa" },
                { "fy-NL", "1f6890d7cffb1926d42642c0785c2faa348cda0ae6d9401d800ade0ab1cb695deac2f4348bfa561d277c4d3d632ed7f80bc03471f9d109d88a80fd15266fb951" },
                { "ga-IE", "313e09500e258ae6dee6feba19970cde4c99f5aa1ebacdeb5f1c501db42dd96676fe837e183edf0c4fff604438f5381729ee9bfe7faacbdb9992db68285ca1ea" },
                { "gd", "6cb91021da30042044e81b60f860e357ef3542bdd6856b7e3abcacb0ed19c7a610a546df5f48dbe65f05422d99adf5d8babbf1b621eb70b336fa3602845229b3" },
                { "gl", "dfe42e0b5e71e81968cc733d41bd3165a179ec56e8c1c1fefb0a9f9bae643d8d2f58dc4d44538bcceef8736903b4133ebd995542391458cfc90f92160787f277" },
                { "gn", "35b9e970380e10f21edf0a8ce5bdbe83e0cc0823561a08ec0584f82a141c8d3428c5a9fe17cf5dfbf161a4c1b0939a936a651b50a7cd8345cb2fa0612272ea84" },
                { "gu-IN", "63e9031675a42e1d535ac1341508cb98cfc39290712bd4d7dff8629b5117bf22cd2e65877c18c62f569d804824e5e6a65f21741853aa157ecda13cc8385ec2d1" },
                { "he", "3c8ee0b100293b6d776d49a56218acd136c19f75fad95d0d9b3e2921cba9eaa4b61968ec7f0f2cec03e5bbd5c0b51c728805ddaf028db6a98fb74db01656a375" },
                { "hi-IN", "6cb40e7f9ec7d2c830d53b8a07233d96816ec82b74050800e014c4fbc3d7bef17bc389cdf94cd4e39eac35ec55c2be094b0a6b1bb26cef979dbe13fd30c22d6e" },
                { "hr", "386f3543ffcbdf1dec2b09f333c01864e4abe084f83c6140a9ca1801ab3ea05a7cb569c1d7c8425eaffcade8fda275c9d14323a5b3dc5fb0210c8dbf2718eb1b" },
                { "hsb", "29049349cd26a8a5d25fd20b0a9a20fb73e185e19fd55288ff829a2736430c80d93bfe336c46c04c5d392a820e1d83209ccf1eaa54ed658d44a26b4ae248e011" },
                { "hu", "2569d20d7bf8026a23f3eaae57a1d51ec585b69bb9e283195c18b6118e19849ae393a5e9aa9c6a7b8aeb6872840cdc9059465d6d5e28c74f214411c317bc3bb7" },
                { "hy-AM", "979091cc7a301bd57b0923283cbf4ffab86b5081008de59d32cc28bf6933dc906d89b027e7b8b2039f97dffdf5d6ffc0b8499756409f137117912c11fb3889d5" },
                { "ia", "d9964b0623add13fc12bc4aac20c42b562922fff1c718fa222a7b7a2bfaeefb725e3be08e382e7b40fb9e570b7f49f32bf1e59552187ccbdd2c40cc28ef1f720" },
                { "id", "14e5cd65d887be1a74d86d6e3e6d41307810d4b587c66d19359898a6b0f29d85f8f7240d000b23fe18a64bf8c2992291c239e172b1da3c7e35c9194b2c19ecd6" },
                { "is", "4cf33e84bac9e8065616a71e56c949d712f0460e6c5177b8af9181396e9075ce19373921da374e713c52b9ccc443ca500b245f2ccb1726ba09b5b5a644243c07" },
                { "it", "db2c60d1861c9ee9963f49590a68eff53b7463a57631b72bdddb46bee2056975eb26d215dc997f07020e7a871553ee9244f4e3568a441be21da7df686473fdc6" },
                { "ja", "d8c8fcc9ee006808cf12d29c6cff04ccf94bfbb01f66c0a7883e9c30a2d0ed2f0440c7dc07d853162ad8f42ffd80fd750cef6d57ac74ebb2cdef0581838e9766" },
                { "ka", "31ef656dbe8799920cb361b730813ec5ea8ff1f497e8351a66aa8c419e9f74666cd8dad53b5034f18834c6975b5f7ae996d7b948bf81a90dfaaa15ddc7ea5e22" },
                { "kab", "b78415eac54e8361715122b54dc9c6bc00b7e1168cb2cb543fc0a9a7f64f0682abf7895dd51c91aca410d1382b0b1e8bbb8877bda5a9093c3c29632c68a7ddf2" },
                { "kk", "668b3956f7fbe6478afbf0e52d15b6ee22d1f95ad2e9c7bde84867c136ef4964875213c407935f1800ed45db20bc49d271feb3895c040690bddf23dcb9a76ebc" },
                { "km", "223c56d26fba035294be16ca22b96d46ed9207c679d6cb329c7c80ab3031340929714614a1578fa02ec036b100c548506fe899cb6a075a786aeec8ba455b2416" },
                { "kn", "1380ec1ced111913b8d3b7945a77760ff120158575f6d34922c92ad09f0b9bd6ee3068d2c123e1a662a17d0305a425d5382734f78dead8885e870cc598d0d9a7" },
                { "ko", "81760bd542221609f4b963691beca11f6e3c2a55d4efd9c8377d057972831e5e0d37ec7d8ae789aba093e1d4140ee4706dd6e7da05811bc8536ba526b6f52058" },
                { "lij", "36e359c18330893bf44391638b2342790103414389e968b242d6329620f9992a4f165feae0482773e6c5a10e4455dee55d5d41e866a3fefecb8a1f79f62dd931" },
                { "lt", "c59106f2694e59c3843461f82ed87b020364118234c67cf54613b0da50d6a1d523bb8ef0dabd9b0098f067e192a257a8cb72950651148db825dfc01c7b9f697f" },
                { "lv", "bd26f0581080e861af5917aa8b45f9ee4482d82765433834d9f642b5763e187373de1d5e3d6ea18bf663ef5dee832ab32b8d24fa3ce8d6860693d5f9c557e785" },
                { "mk", "c55cefe3a5effda90261b072123cd22e6dd7f361c64e3f64bd99f38995d5b0cb2f88bb15675d27b6696dc6e5c75b37374887ee698248d5a1fe61583cd1a0dd16" },
                { "mr", "40e935ad966712ca2c76506e04123d873c45937c4195440cb63172f69dbd43a34b2665db12723ca75a35e1a504f9aaef08ef523671d53becace1326e148b0748" },
                { "ms", "f95d770f5b880b22d4ecdf4f9d6d445fabdd939aabe3600058359cda4a7e7856e417dd761c474c2838a1c7b367e378527455b157df164370e37f6e9e901b4aeb" },
                { "my", "92e8b15ab77fe33fe69595c690130238a5ff5c485526f175cc00c990a71edb0dd31993d562bb0adcb8677d9b0a7920b23327245835505a019800da0d8d7f3680" },
                { "nb-NO", "99ae7ecd860b778b4359ea891dbf8b08464e6646751ed7f2747b343d2311e1d4d94f477e69b8e4cd026d6cd8b0fbbd029f3919bc56cd8d0dcedce75d9ed54ff0" },
                { "ne-NP", "9c343f1414395c8d22cbd0bd0914e7a3cb292f36a87f1247bc36167c8b616949743c1657df5370da7f36350507b902118b5100382471ce59af5de56bf6a461d0" },
                { "nl", "0688adfcb3157f0832fd4b6df43d5bcf8a4dd1907ceac65f87b1b66c1e12f8aa87b4f35861ee8506398bb195edd2545f8cdcd9b9a0c8a0e5c2ba6eb3a7bdd07e" },
                { "nn-NO", "032108f6164d0fb57bd27953b5338447e3143653debb7faa9afa27ee53e7d16efd6b8b6e1d555c49ccf8a58b7ad1eb486afeefcb53076a9acc00bd808381818a" },
                { "oc", "0cddbb7bd3f8cb354664a6d5025f9954bd5c55f833b41970853e3279f384607bdb99a14f0814059385898d3a0ca93787599aceedd62a170270ae75a317fe289e" },
                { "pa-IN", "af38b4c11e7d2fe0ab2ffc6ca46c84c49099f6aa6f60fbdb82da0e14b4823bcf11369019917bca1e02b0dcf0679c7dd1538ce824047c89cdb459de0819353586" },
                { "pl", "77c72f71bcc595fad4dabd473ff3db0f944189de147a7d48ce8452c475addaed80d1b727d6b4b1ca8c75d86328f227181bceeb1784f006d82a781c6ec6381f25" },
                { "pt-BR", "869c7f161b91311cc29aa00a7204d79b92de251d0d9a813b327aa57506eb3be011e9429c328b3329a7d600de62ea11137eafae8781084ea699fa52448b5e84ea" },
                { "pt-PT", "367925fa43fd66edd8d70f9df66492ef45033b47d75510859f8aa83b72a47af2dbb27581ccf37d5e8617acf8f8553d242dc50a82a6657dd4a164591504b3ffb8" },
                { "rm", "980b994810fa6caec5962848f7274996b44b4157bbc27056dfcdc6dc61ff34e7b70e26b11ec8104a166298976d48ad5bc7d271d9e4c1bbec98de7212bb12b6c4" },
                { "ro", "f7378948c506f289260a1b43a7f7ea80952774411a60cb2a87c732c23fe57423c2bf0dfcb9f0bd2af72c4f6217d8534f8bc0646fe99ab3ce62e37eb59124fc6b" },
                { "ru", "8243c04540900df5d0de0cc85b2af9b725ab7711ecc75026dbcfd903d2f515187ccd2e306182f2392ccd84332ef597ac23a55b46128ac28b3aabaefc36dc4895" },
                { "sc", "48910037279c2c0256d983bcc8ce1e7b2aaef723571bb89592b0b20efa2d8cf6a1f78ee49091d003772b43fa40b930607cbc256d811b9e0fceaabda4d0217e5f" },
                { "sco", "991279c0928fbf1a7a3b32249a069dc5e9541340dc5448a3f68bcdcd73993d655e2353a06293e7a75d384b3c864ca20b2d29f8617344cd21b69ea6cfdf24dd35" },
                { "si", "a378cefd342ed5ab93f8c7f73f3fff57aea1f399fd83cdd8202a2994c7d5a73219a432466e548a1cacfc94716d7a2c9f9d7146631d48cafaccb4f9dbd78b1f23" },
                { "sk", "8d2dfc4af4f2703a7a0f39a5a084ce83b22062767d8d518563c9e69ef9628876f3ba8da7b2b0a85f21be2428dcfcd1a3f27c9e69869500f875affcd15dcc62a6" },
                { "sl", "ebf81f2e80d75283252a49124892448c0a30ac63e5e6b152d0084f5dc8488f78f4356e4def7983ef3859cbf102d2b40d10f5c3c06b05edccb6863958af1443c5" },
                { "son", "26f9742556c64391d5e1fe77aaf8592b4c28bb099e57b5a9500e5e092b0e4e1aa86c0f6d095a6aeede1a2f94b02d02f89c45e67dda720ad48ec77e21f33915ba" },
                { "sq", "369f75dfb04d3526c68993ba092c351db1d6af0e9d7e8f6875ef34891ca9ad78b13744c7bbc17b27e909f41929aa59a7714d6d5ef1c29971da313882279501fa" },
                { "sr", "7547cb36fc6595ed108977069b893edc9df48fab3a425b58377d50966398cd448eec8c25c4fa7ae7ade0891a03b44adadbf83fc8d460428d16b57bc1ef2a30c6" },
                { "sv-SE", "eb549785ba0d717baa6822834bd6471946641088e75bfbdc7afbdac68244e8990bd669040d95d253ca817ba963dfa53e91cf0b48b57b35e0f05906447b33c678" },
                { "szl", "6cb9e9d47f2f7e4790f318aa934f2c66a99d45e6f70ea8e8f13a047cfe4d220304af63885c809a6d3dda45f6148e2021f2ff5ceea88309a0ec9952e20130819d" },
                { "ta", "01eda9f0205db21cb8f5503e68c0db047543e06b7f79fead9dbe6a65c5cc63a70dc66f3836c902d451842d7b41888df7b89e470d7351681e937d3bdcabc4c85d" },
                { "te", "f038762e98d2981cdabc3ff3a06a7d279fbac410ae3576192071a1425d0c1d77bd2481f99a2677e878507ab054396f61fc26062199d1fbaa23dbb3878de5d80b" },
                { "tg", "2d6ff0af338ac9ecab5e10f934b517d8453f818bc37dab0a9b3c9342e0f84bbb6a251d099124d698180474ad1b022d6e89f5bd3edd5f5bc8b3b6cd44b400ed42" },
                { "th", "d8a801f1c91b355714799c00fd9c9319246e761c4b17a28b1edbc5e5ac93447dea1bf9948e67f95b00d755a1cf3dfd9a35d294ba778aad26edb95c29ba83a587" },
                { "tl", "3e9ae57b473d32c77d086e4799192f91888fdde32776cef6098cef94cff699ed2faffa5280b6fd7d0659b43ea231e5475ececb775bbe4a746907c6c148d350eb" },
                { "tr", "31741db884ad3f4208220f7dcba6a9e4d49de38498f976eaf664fec8c9438317a13a1c3e38db1591b6bc1ff2f9f2da775cfcacf63fa7e10396f0ab529a08cddf" },
                { "trs", "3ddfe22fd49d3a3c88d2bc15a430db58a59d8c316e00c7c073069a76d5086efcaab6e31546fde538eabe7f83ac9e526391dcf82e57a7195ab65ce705e97e51b3" },
                { "uk", "0ddfc4a5519e0b1cba4d1f9e14735dfdf8b68dbc2ae2dce37836f1f4cf8f0491adb88f4b80a83c5d2e46cca5ffe05c2a315fdcbd95c95497d74f402465467ca6" },
                { "ur", "d4b105a35d0b5d1d689d6c7425b7fba2ae7bc8082fa22fabf4c8b04889391543766184c2c0004c249a1628e25928df1ada71ce8e0649c832ee6a96713a731e97" },
                { "uz", "e23ad2d7502c4c7f794f0e2a8d49b0ce17694ddc4d513ca81243c0655d82bb2e3f45f135adf60c56dbb9dea69e9a1e3a1aab41731c02ee1a9fc2025167c5b43d" },
                { "vi", "c188a477496b5a00b8cbd2ae3e362d550be00e0e8f676624646eada7a2dcee0e881c09ff11cf4a31a3666533b9d508ceaefcbdaeb51ccc8760f999addc9d1530" },
                { "xh", "01f808810b726f79a09c001c7d899f621b192115ef1d4f19e48c0e3f45756490075735edec6c6b1c3e014faef4847943467746555c6ba28f4a060954501e6b7f" },
                { "zh-CN", "4d50bc123d41fce95b55d404b10b31535e097f1c8a9a80d3b9c146ee99f1bd815778d6cd29308051221c3ba35c4b7e930d648d45af5758d3615073fdd304e1b8" },
                { "zh-TW", "8ea673fea07bd4fe3e408c66bb4c0311978b2722fb4de9b1206ae1f3d0de5fb86658f813715d988b9f41131c7acfc6269b1095160a446ae81a115cec708fab54" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            return knownChecksums32Bit().Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Firefox Developer Edition (" + languageCode + ")",
                currentVersion,
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win64/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma")
                );
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "firefox-aurora", "firefox-aurora-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox Developer Edition.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://ftp.mozilla.org/pub/devedition/releases/";

            string htmlContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                htmlContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                return null;
            }

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            var versions = new List<QuartetAurora>();
            var regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
            MatchCollection matches = regEx.Matches(htmlContent);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    versions.Add(new QuartetAurora(match.Groups[1].Value));
                }
            } // foreach
            versions.Sort();
            if (versions.Count > 0)
            {
                return versions[versions.Count - 1].full();
            }
            else
                return null;
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32 bit and 64 bit (in that order), if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/devedition/releases/60.0b9/SHA512SUMS
             * Common lines look like
             * "7d2caf5e18....2aa76f2  win64/en-GB/Firefox Setup 60.0b9.exe"
             */

            logger.Debug("Determining newest checksums of Firefox Developer Edition (" + languageCode + ")...");
            string sha512SumsContent;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                var client = HttpClientProvider.Provide();
                try
                {
                    var task = client.GetStringAsync(url);
                    task.Wait();
                    sha512SumsContent = task.Result;
                    if (newerVersion == currentVersion)
                    {
                        checksumsText = sha512SumsContent;
                    }
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer"
                        + " version of Firefox Developer Edition (" + languageCode + "): " + ex.Message);
                    return null;
                }
            } // else
            if (newerVersion == currentVersion)
            {
                if (cs64 == null || cs32 == null)
                {
                    fillChecksumDictionaries();
                }
                if (cs64 != null && cs32 != null && cs32.ContainsKey(languageCode) && cs64.ContainsKey(languageCode))
                {
                    return new string[2] { cs32[languageCode], cs64[languageCode] };
                }
            }
            var sums = new List<string>();
            foreach (var bits in new string[] { "32", "64" })
            {
                // look for line with the correct data
                var reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
                    + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
                Match matchChecksum = reChecksum.Match(sha512SumsContent);
                if (!matchChecksum.Success)
                    return null;
                // checksum is the first 128 characters of the match
                sums.Add(matchChecksum.Value[..128]);
            } // foreach
            // return list as array
            return sums.ToArray();
        }


        /// <summary>
        /// Takes the plain text from the checksum file (if already present) and extracts checksums from that file into a dictionary.
        /// </summary>
        private static void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32 bit
                    var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value[..128]);
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64 bit
                    var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value[..128]);
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether or not the method searchForNewer() is implemented.
        /// </summary>
        /// <returns>Returns true, if searchForNewer() is implemented for that
        /// class. Returns false, if not. Calling searchForNewer() may throw an
        /// exception in the later case.</returns>
        public override bool implementsSearchForNewer()
        {
            return true;
        }


        /// <summary>
        /// Looks for newer versions of the software than the currently known version.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the information
        /// that was retrieved from the net.</returns>
        public override AvailableSoftware searchForNewer()
        {
            logger.Info("Searching for newer version of Firefox Developer Edition (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            // If versions match, we can return the current information.
            var currentInfo = knownInfo();
            if (newerVersion == currentInfo.newestVersion)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if ((null == newerChecksums) || (newerChecksums.Length != 2)
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
                // fallback to known information
                return null;
            // replace all stuff
            string oldVersion = currentInfo.newestVersion;
            currentInfo.newestVersion = newerVersion;
            currentInfo.install32Bit.downloadUrl = currentInfo.install32Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install32Bit.checksum = newerChecksums[0];
            currentInfo.install64Bit.downloadUrl = currentInfo.install64Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install64Bit.checksum = newerChecksums[1];
            return currentInfo;
        }


        /// <summary>
        /// Lists names of processes that might block an update, e.g. because
        /// the application cannot be updated while it is running.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a list of process names that block the upgrade.</returns>
        public override List<string> blockerProcesses(DetectedSoftware detected)
        {
            return new List<string>();
        }


        /// <summary>
        /// language code for the Firefox Developer Edition version
        /// </summary>
        private readonly string languageCode;


        /// <summary>
        /// checksum for the 32 bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private readonly string checksum64Bit;


        /// <summary>
        /// static variable that contains the text from the checksums file
        /// </summary>
        private static string checksumsText = null;

        /// <summary>
        /// dictionary of known checksums for 32 bit versions (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs32 = null;

        /// <summary>
        /// dictionary of known checksums for 64 bit version (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs64 = null;
    } // class
} // namespace
