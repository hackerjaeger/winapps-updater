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
        private const string currentVersion = "109.0b9";

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
            // https://ftp.mozilla.org/pub/devedition/releases/109.0b9/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "07c6fd6358c397868f38ce22f493d80ebcc726a6b9e3ac829d3250edd36d67fbb5f7213e528d079ddd4c79c0f70ee7d3fc854e6eaf37c8057e1e4a0b4b1726dc" },
                { "af", "82dd16ac4295aefc43edcaf7efbeb574115d3e998bdfd246d046b9fc2b8aadf7bbe58098ab1142b62d2ffd9e5d377315d082a4e341491278ff247dee4efcae24" },
                { "an", "66b66df1bf37f9b0b7f1a3949c8a4ac7036e1def1833c554f40c447e64aba072b099510be2b3bcee9759872ea54db236b5b24689bed3a088dfafe0a46aabe83c" },
                { "ar", "c13d77febb1728e821eab7ea48c4d17814740203114a3e075e769d8ba0baa2e86078f16e4fd01a7ee9626d4c14b966ee069e5e92797a683de5784c3cddcff337" },
                { "ast", "94a4fa1a26a4827c1a2ef277062f79752e17e991b14b980f1d1802c4601b0e1d4d48e5b8ff50ccd050abfbf60c8c9c935e58dc81b71f5287a0e5597b4dd54660" },
                { "az", "1942fd76d08f2219e2265c93be26b3f36296bd5ba8982e4254aaeccc6ce62b92894edfbe86eb971199f92f1e06f905c358fea8e0be6f31bd142c094d124eccb8" },
                { "be", "f5aad2f56ae245e05d971fbf27dbcb35873d178e09ef5a69e5e4c7222618b1182e33a43263159761c84d9cae8f69b902fb1682425a6162f09042add19e334adb" },
                { "bg", "56d94978f206a582f242873ce0f4c2231777ba4b067427ec174d9dd3c9f87f5e50a5afb4b81f1181d7bf6a62048d52671424b0cc4c7a784beac7b014c0892bf8" },
                { "bn", "91dd96d8fdbf2e190d50fbe15fafc8a1866bfdd76e7b0c4175b34b982593ff6e3b06c583a9183dfbf824dabc0ae23938cdfc6c667f02e5e4f906468c23134150" },
                { "br", "d1e48ae8f2facd152572a5ad929333c9bcacc2a185137de9cdd0ee756cbecf2317777f4caba278a09f4efec341b438fda244fe8c0e95c8770c234ddff48b52c4" },
                { "bs", "01bf8d69f4401cd14ba1e24e96adbb80488f1c2c79ef9b21ea197d10f35eb5cc4964cdc81506cef64dcb7196d9ee5af99eed8d0f91ef60ce3d304ca3e42c95f3" },
                { "ca", "4741d3b20a0f7638afcf82c5683addafcfb2342de19a8efea473b3b0e3f87b7fffbbfa7d92d790bb0809ff0114b4167b802994691c66dfd653346c5df26e723c" },
                { "cak", "6053a2b819cc95caf8b513eca555b27baaaaa9d55f7e9b4ea8b240854881c7bde580e942eea15afb3e71574c3604c4e50741ac4a1e32481cec0bcde17bb17a5a" },
                { "cs", "cbe0b4cf1554486beeadd0862eb7aeae69f75cb75d533c8dc8e728c600fb35326e6b7da598f9ca1f03c189bc7bc8e2da22d8f32f8bd5eea3757e6583866a793b" },
                { "cy", "a9efc2dab5c9d81254cf89f629fd9c096c1cad3ada3c628adaa4db9ceb06b2b18a059314769510534df1af727fa358a5929ee1228ab87d73e299a5fd813c9cf7" },
                { "da", "40961834061cf11779d94fd2545cf131dab5ff700f7d9daf2aff08725ef31a1047a3005503ff8553ca0fed3eacb4c07848968a8a9d343f99389e2fbb33f79b38" },
                { "de", "c047fa76a13f7735935352d6793d7c44f3ce86c1f0888b4089bb5c5dda1d802ca3b3f48d38c0a8f8716c3469bb09f56d04bd2e59d3ae45ca4a29d60b3bcc95d2" },
                { "dsb", "37a8ca8ad017f013aec6bb260db3ae644a502939952921ec9863c17f671249f4231d63c79a3c8d9fe6a3683cfe1748a6fa40d10f36b49938289f34e5d1eef207" },
                { "el", "46b4fd04d4d11a52d28d9583dcd898285a54bee347c8de70fff317749abb2de3c02307946276aca968333961c1f61dabdd36bbfe1b1fca09e89ef7815764bf37" },
                { "en-CA", "fc21870b2e08899ffb126c3f65f317f3b54d412a7deed4741d9e212645fb676d0fccffff2e5ac35b7c27906fc4a50a4b860127f734577ec1a0933f5efce78bb2" },
                { "en-GB", "ba0e3eb503817263ee7c77eea01c488944b75ded15fb29da0637e90c4823b42f82aa19188223ec2e24a4bbe5b9471a279162b2bfff7367bed46e3404fe42ca1b" },
                { "en-US", "bb5f4176c15d97766d91006f67aa52ea4fe87d2152adfba8c4c5af8ecb8d162a2a8ff7e4051a41026247ef8cbca626b9686ed39df424d7fdcde354a907605c68" },
                { "eo", "48f021af6992dc8c09cf494122c02297adc45322ec12bba9fd038018b6cd9029bd33aeaa66b6c3373028461a86cddc6ce489e68def4678a5869a40b521e09b17" },
                { "es-AR", "2d9239a4872b3117b78e8d4d512099dfe9237babcffc8b1dead6b50dda1da7f713d5ebaa9f16bc8a180e73ef0246e7d67309224350ddd9ffc547671d716b0bbc" },
                { "es-CL", "ffee8139e9de216797860ba89045f6d0ac0553745eb00807fc9cc0de57f23501100542c766ca0f2d1d9c6369809db6be45993e9cbbd2e9efc2d42c0271666346" },
                { "es-ES", "bb95bf471c6ce38a1f12d0f353e2310a56b4ca62fae3fdf80b1f007ad6fbefa4c88d662a6e0573c3762a6069c353a7d77c0b2c22be29fbd33eed7b08330c3314" },
                { "es-MX", "9000d7f259d7c915d2674e6791151ae0db078d54e8ba77fcbbf3f05046b2d38570061cdde7e6a0828cb2e330d658900ddf1a3b54b7cc6bdaaa36cf09e9f5f6ee" },
                { "et", "7a92f829ebe4ed9d761beaccde650680ff95b6eab1d361fe5ff22b7ad474a3c456bab32a57d606e67ca40f7d8b321447cbe9f8ea6e714999ee1d8150c95e9097" },
                { "eu", "f77a29e050e93b725573461870ecd4a386369f0c59f2a40d1337ad84c861676ff0928d6c77c1d4adbdd41ea25078dceab55c317ca2b79e8acfc6aeffec8ed25c" },
                { "fa", "ff3e6b6ad7cfb841b9d7ce04d538c782c3c6bf04a0846d1933e25a76159dc8410a96832747becc609b384ed788617652f29b1e5bfd9438e27b2914085f4b26a9" },
                { "ff", "8f5c024729e9755ffc2e4d6af4d6e4c74b3d5b304e0805a37792622e11b08e1d4d801b06ef9903d39d7bccc0664f38edc24ab40b96c228f2996dbed6f50d0665" },
                { "fi", "1b979496d5713acf3387bbb96f2f85641a0d1d550f5df217a1c9fbbf65b868121c7779bd6648abb1bf9d6dfa9c605cd88d5a1381b3b60c46066024ce4e65e91b" },
                { "fr", "103f06af6cf7c0d3730a0579298b3aa1d401ea5121c6aa1e4e739d3a789544fe9a368d080aedbb29c320f37bdda149e24767b8b6dc166cd4029003ee607424f1" },
                { "fy-NL", "9c18cb8c0731478ab56e9e43e5f5fda250e21a13209aef50ad099d32d6e334b093ab46a2db522772db7f617ca00c6af0793aa2bdcd5a4a06fb3d6e7cd7ce62b5" },
                { "ga-IE", "c014f7b53e54c0ddb2387173bbe83e62416ab7e2f7d1c95e5e10b0533e8bb8d7e54f384dd364526761635f643215bed7334b08f84c93b606a82874df534f3752" },
                { "gd", "58624d71da48e9e74a61e7964a34b1a6dde23274a51343c773dc081f0ad70f6bb7690dc5d5142a7853e7cc876b08a9fb79c1c3676ab464f3926ed38e93a5fa28" },
                { "gl", "128ff0efeb530c978302221c46ed930b2ef0b907ad7a417c1199ac09a1761e0e2b8b5b4beb0cfc5ff70a92bd78c1e83b780cfb9bc4ab5aca026d132fafc71005" },
                { "gn", "296a5c0a88c75ea9fd288c8c0d59a9c812f34383b29f56f64db19d7a8ab73c289a0bfd4c2d571a2d0e7a5326bb3c188a395a1e86204f1ca262f74f8159f616a6" },
                { "gu-IN", "f1408e313d8310c8d25c2689be7716f4c1dc6fe5e9b795e61a1f10e033fcf81ca1d1270d5bba21cdd0e9912c4a58ebe80fe11eba79d6f2210baba193b99c3afe" },
                { "he", "a5d613f7f5ad1d33926d06285eb4eb1282b974bfb41eef7fdcc03721bdff575caa35b2a7616aa4109c6ecdc52f19eb396d560a1645ae99e0e9c2ac575b20dde2" },
                { "hi-IN", "ef74981c4833916e67f13ef30ba827676e2d8b4c6ffdae545dc43c4ad5684031e0fad5daac0c69f1e34d460c761a002cee3233f8c5bdd16afc1f4135cd3acc27" },
                { "hr", "49b2ed9fadd414e23f08e0e74d3aec7c6fb9eb9d13b7d7c134efe4ab952d24ebb91cea5752ff95795880e1bfe7a44f930d99dd9b4b4c7d1b5c893b8d1edceaa1" },
                { "hsb", "cb1fd160de977beef6fab7ceb2267748cb663ff34e2fd079e7a49c63e5cbddfa49bd3bd3f2d57b40887d312035540af290a4757c33589c60fc51fbeffcdfc902" },
                { "hu", "26b7960936ed9d01f8b27b7317324aaf1a416df1e540de78f33460712421946e93555b2f90035a45dbbc79b0c6f9c6f9b7239a6a6cbd0c597f00cf309ffec863" },
                { "hy-AM", "90c0abccec904076fa4aad4d3e48f2fd80fc6a300db362bb04420d219b44de8feccd458d6e884a98896d96491b185afbcd0b69ab2869ea6c5c702b448ed0411f" },
                { "ia", "8dd02af40bada50735e0fe899ad5d7e54aef1d9be640f8a100c593861c145fcfeea61bd69b304d91d4f4612f0e347f0de8ba778e866d06083dbbd2f4cf37b215" },
                { "id", "217c7d429e2cee048969512f894e106baecbf1a081ad6bee006270405449e1eaffa6bcca6038f18190f737289cd8ba00e9d2f0a06d32f3a58965edb733960c6c" },
                { "is", "3a5aae13b8c912023f29d5bd66debc25afa00ff3cdb4670a82e6c5812f32c80c55355cb2f7d97edf5ead9fa9648687190d1307da9b9b1f8a572feea7b77fad4a" },
                { "it", "bf388b6ed7f69be2b220fc69d329b60542572ca8886657355cfab26f898919e1c893e9e8405e64d63d909e53bcfac8a99d5e6ef363df2af20d0bdf0ed95dbf61" },
                { "ja", "811b954ac0e7a915b9e5f5e830d53beb09e58b76a310eacd6e82621cbdc55f48aeef4425e1305790f06eeb783a3620f41b64f57b8da45e948d5bc03fa0cbad91" },
                { "ka", "819787424d4efb929b4322c5aaded8e5a61e7ee743348cfcdbba81b7d0fd4182519722b8c6d920419368df46e5d5a3815c4dcee990c504645703e6dfe695b126" },
                { "kab", "6c913756f79b542ecc4166482283c80545888ef3abf286eb6c751d52353b2ad250df608ada9a68d2575e63608e3a60edcf818ef6c0087139b211b8182b74fb21" },
                { "kk", "13dd669c3e5146853f6e89d4ace8c7dc35f289c7fda273379c2801f6f8118aff042b0bf434542a27e7b030c6f4fc42ca348ae077a246ec870d033b5a4f05fc13" },
                { "km", "31485a69202945f78cfeaad1ca014b73ec2bc54d5bd7bdea2e01dcfdb5d14e2705d2aad800e9c2e7ffb87450057987c115e58a2a1d4255b702dbc1f53467e97b" },
                { "kn", "d914b119767621aaa74e948c7481060103390024253c7cddb0e659d85aa5a4eca664e1dd39f531dc8f9fe969e50bcdfe99e81aae98ccaf60a193e1b981978222" },
                { "ko", "60cf03123cab101b68840d7513566e66fd10f49f29414ce187698bf747473c2cc9558ba890abddc90bc2e5a485dcfb33d3fe19600e438609ec6293f3573c2e50" },
                { "lij", "dc23086cade851cdb289d7a6521c3fe7ec3eafe052fc699f1f2aa8893c649a7d4a8b7e49f430e90fce65e81962a992746460b2956a2854de029f384c324474b4" },
                { "lt", "f43ecc10ef5fac6dab5e3d62c3dc5f092edfa75ecdd4e36f3f0d958176d8caad294adadfe25d7f92300150ec3463092e445359580b613a538ea2ae025e781625" },
                { "lv", "fbd85d41c5b1e3ffeedee1493b54eb3bfc292514d986677567dc464e59c09a91f957b0bcf6bc6c77321c4cf2210a09900200242d7ae8ec05ee07c0ee680eab1a" },
                { "mk", "112d4ce54cbf608277acc783d1ec7a3cc421f6ec4c59fa0d45adc334492e9b131c260e5a21b37f7fbe2471451fe900ac424885dc68a16f68ff42f6414078c3f6" },
                { "mr", "d534643cefb39b7622a1feb092816449e9dad91eccfcc80ecf4b379850e5090fa6989c8a9252bb42077fb801221f2b6187e0094bfad67f23758ae02e45be0383" },
                { "ms", "20cb3364b23c9c18897871b7735150bae56f5621052a1f615dde929443fa54920e72e7e781ee14354aae5589024149b9a5498074cbdf4707bb4c72fdeb9d3d20" },
                { "my", "e118e3960934c3b0333782b3a9b4b6132295cb193766d0c9068a291a052f7a006d4f119d322f32585cb1767c6d8ac71d2ce00f9f51713261491c3b96625325d3" },
                { "nb-NO", "689832abba7cd422593f93fed5745ffb848fa08e0c17d24638b53e9da491c693994f1a5d6406f1764b960d6a4889b5f04f0a4b1860663f28e6f8e7e1a474e41f" },
                { "ne-NP", "90ca27b6e1bc43181cd547857eca0f03d36308dc46ca1d6340ea761368c2abaeb5939f0a8b740d4d7725723048f6bb2653cf8439cc59fc74d44487f4606164b1" },
                { "nl", "b05184f64ddc6a03e88cad8b2bd784acdc5c257fc3fd84eac9b2db5ca1dde72920ac57e96e1107299849ab8b1f479f781811883598ffb0065481845c29f48caa" },
                { "nn-NO", "7a4bafd36e4bf6069746c85ca5ed963a8a4e6b7a81cfe73f62feb3a4000e2660920522f05d998092ae0e9e15a6cd2981c16f83ace86517ad1cc2247a2f6f22db" },
                { "oc", "157d7e0d87691517ca1ee187db94960e78de4196643c575ea9600adad5ecfb4f36d3a79d2a2a8612c014f16983aa5f4f38db2687fb6a012eae915f713ee129d6" },
                { "pa-IN", "63c4d0b32193d2df3154f9c3c0ccfe1adcd7aeb57d55b6739684913001552e09b84ec60756d5bb6eef249daa9ffcee28ce8d9f740afcf7065d3ac8da5f473466" },
                { "pl", "cb4408169d1a1bccd3de581bc7561ad24b8f54a73ce73afb44dbbc20d7ee8a79dbacce170991025d2592578a17585c27f98c661e34655896672311ade4d79054" },
                { "pt-BR", "79266e011672adb23fb4922afd05b85b77035a7bd0b80b0d2d863967b4185fff98b42b4fa8e86f10ab6206e9501fe2bb7a3be6bef1a671ace42a159158d76b8d" },
                { "pt-PT", "47c99e622a67366cc6c1fe94c86a60951aaa13edc284dc3ebe7bf37805b354bfc51a713298f35fdc26e40d0ca50178320f0e972e99cbef4cf13381ce1b064b62" },
                { "rm", "a868a9f5e153023d618c224f8dd98eca42900e4a41833d53efafb728999e456c985367923167d0fd2e26fe9c5e8350fac716e5102182f27c0343711e6bcecebd" },
                { "ro", "75925e8c2f388a4a7a6de99f3517d93e3dd84f7daf171990a35a2fd0f63c6cc4af657535f0d38324deaf3862ebc53ac634a30f20c8a8727cbbc3e06227bc02ce" },
                { "ru", "ed89ec63be4a7ca4a1c4369fe624659ec59366ea176a827fac398533c47183008c2ad03c1d9456f628127cc0b92104a98b7d5cb306705cedb7e800930166c978" },
                { "sco", "53fc005541676cc1fa4eb203daa50c0945031ccde86423a255974d48401728d1dce11078d10d1f9bc43f4268e6e292ef4744bc9953b8eae3d11a18c000b7831a" },
                { "si", "abd01edbfc4169d05ff4ac1b23a8fec75bde115f96b20963e3bb60e4127a628b59bbcbea66b58c2710e29947cbdb7b4d38bb2ad6a0afa259a2ae0e1d8364b559" },
                { "sk", "7a2e439c49b69b9a3d6ba92d7b03fd11d1b96f469a2e47f49c85b468f624a2d62d39215760958c5f5cc4002f1003a755d7b5a47921751c61c2478411ca9ff6a3" },
                { "sl", "77ffad2af57278a879d6933b5dd65e57081c0da90e95ee383caf422c2f2fb14d68b8617925b3494a17b5f889af1ad1707690d3d526974f98b20cbfc97a8f1e25" },
                { "son", "a1b652edf435b9a7d9de3a7b3ae84e44e2bb3acc5fd53648b3bce3a3c5e07559b3b4e88c0d3b786a1bc5ebefc06e9a75231094669a8d4e1abf0c91066761b9e3" },
                { "sq", "109d12f94110d69170dac1e5485144b06c4c7a474cbff7d74412f76e18516efb9a6be72b7935e121ddd157b4136b7571c01e5508a734c704e97dbbf106e8d39a" },
                { "sr", "91e4fe29811a70360540fa5d99e11552bc78c45a4b38ce9916de73df2c5b4155fbaabdeae0fd9dccf847fec92ee71d31e39e9d72bbcad726596dbf99cff03e44" },
                { "sv-SE", "0e3aebcc51eca1acce01d02623e7b4d67192d68bdb33e1e15865b6193c1d3f096d499bfab582b3aae626332be25a54f6969db2bfa837bca6a404c57269a2b3df" },
                { "szl", "18d80ef22882646883d4f0bf78141b73ecfb86450004ca32ea14e3b82b62e256f3d521f8fb44eada4d7bec38fe42152104c70334f798a588e94d11ba210f030a" },
                { "ta", "7faf88cfc65a716a9419a6364a242a7900bb8cbe8851b901be262a8f45d36da3c40acc3e3988934d0d20382741504f71bf1c6cc2cf9b426e23a69612c0358967" },
                { "te", "8cf1753008d671fff02ca4e678c5bbb8ac71934f72ffa694c80b8e8b9d39e093993151ceee6457fa3d1212301b691a39fe9adc771b3b91445113e9cfcae7f374" },
                { "th", "535cfd1aff3479509f70a4902421367d6af849c2ea52bcedc6f9f58504d842410933457129fced515ce0aef80cdbaf47ceded9250e2f443d8a9456277b687c45" },
                { "tl", "299322b14045fbe03f3cbf379e9d60821a64123846138e93c1dbad7e95d0b92371afae17c0b2ac02cc001a3cf46d76d61dc575a0c0feddd33b62787b18a8f4ee" },
                { "tr", "04c7ed094c0f8440ff1bef01f60799f38e183a5816b318bd4a57e88f409aed7d4ce6eefadd3ebeca11385b6a28b5fbed67a9fec0f9d6de2df8e62f95088369ec" },
                { "trs", "2557e0e77bd97b21dbeccb89ebd1516a6d91ecb916956a40172e7a774b00355d46ce2da2c51ce1571b60c51aa01295a3420a93609a6a0a75252de19027320a4a" },
                { "uk", "d0e8b4bbf47f91b26000c7f450e06724aad6fb1fd85517d0e2165aa996e8c998cf69df02189fddf935651ffae4fe89328810c349dd249fde63f6e865c0966890" },
                { "ur", "b4ff4601a7ab0003ecfd10a7d4edcd7ab6a7ae4a063484cc660a6c44bc436ed1eb93c08a59596f561d86d07cc33d93bcab10d579810fe3bb425e128b061f8eea" },
                { "uz", "75e2c67f73e85844d3b7113e159339a86fe62edfd9322344f5a6a0bdff5a21314f99a67be4d672c22a8f1966694f267ac36be62eca5c5a205258f5c8ddd17736" },
                { "vi", "c4d47c07cfe35c5f8c09e2aeda63796122799b61e3c28b8c2003ca725077f70079e2115137f0c82e013913c3bd917197781d77e7a66ced773c73f0c0a7804fc2" },
                { "xh", "c6d8c530dc398c83f78898863a319af000e25a6579e77b63a9e3965e5c1a44a7cecec7d1b3db9abda68833b83759dc2c8a64b775c1f7219f9d450462b1f14a4e" },
                { "zh-CN", "ee00aad6b579a455575f07fb62b7269ce6b3e0767710901714c0727bd640c0890a7f8e706a86c1e6f843d10d97989853ec9754b19b4d99773d83cd5940d72a39" },
                { "zh-TW", "02304891518edceb77ad0fce964ff4c24d86d01a46fa7a708f29263e026f8065cf813977529ee70bdbe399378a3f75f6db26b237bb24cf9d5f0f5bb62ecf64e8" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/109.0b9/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "6e05e724312ad64147ae6db26f9d9914ea2cc1299c3f644745f32e83c1529907ebb93b5234eb6eae12d088204cb254e0bcaed97046dbb4463fcffb96a268e5f0" },
                { "af", "982dd6321e6f42608110a19c5249ab6c787d4e8c22342cc359de88b2d727b2f2dd0ec647af7b96b2a294fda69e4d3de45f90c4918c403c53203570762933a05b" },
                { "an", "1006e602ae4c506e2a90849d6addb49efb453faadc9294b14f10c609c3cf7e5a9cdb252664f5aac17e3ccc76b39e38e27f0c49df47ee63a7028998f6b2ce4280" },
                { "ar", "92c98c1ef721e59008d7e36267e9fba6771c150f7c2fe31a6a5edd13b17107409277e884e73bb05327fb45c8e4dddbb0f6192979379d6e6bac87510dd2168dc2" },
                { "ast", "bcdf42a4802aa93e4826975a8d2e3b25925b049d953d80d03f086b54353462d840e8b41ab4b3f7d345b16bd7968844300ac1a063cccc2a6318f761158b9e6b06" },
                { "az", "c76954a96e5b9609199c9d1509bd4e68116b6f35f66a55b93309e459745c23103269e06729cb4a6aba50d7d18a7bdcf459344fef21a3407938007582ae97f468" },
                { "be", "31f34434ca67de2ccb75af18d3cc708f52fc1adc6bbe6d0b13d3b248847c6a56ab2a92b519b5056ee0a720dac03f660dfb71c472b1ddb7043dc7059cf13739f0" },
                { "bg", "15d8fe0f98c668f24eecd5698a32baa4520c3c76eff84eac0abe548a118354d4a331f722b028ea71038e7bc2fa083cca22909dc5e2b7108fc441d3972fb8c205" },
                { "bn", "39603377781e61c0fd4cb841a19b64797eab646cfae193d4b7eb396d0f69b99da65aac670b8c5488fca8b9fb32bd0a86f85e0e159cf71c43306c9bdcdcb9c591" },
                { "br", "f3fd05b097f9720f32cfdacbc6252351b4e55fbf9fbe483c91b20756724e850be03f187535ba682cf8ea89584164d5abb64c19da59ed9ea62f1d2ecda9b217fb" },
                { "bs", "ccd2d3dace8de0d687317718f40d5123af7836049e97fca1df085cf48e8355243bb457d56fe7552293f009a8a1592d7d5c102e9079f66e2d48e6a3c11712e0f6" },
                { "ca", "367f501536ac83ad32ab6cbb78e385a544a0363293f2e07826e105fb2645167e9fc59d8c9add96148078ae67140d28f07ae244ade242b03646aab72d335a61cc" },
                { "cak", "2626aa8bbf5fc4f90459897e057b573bca0ac0e2bb83238ceb80ecade720332bd6be032a99e0465effe84a610c2215ee20953acdfdfd88fc89bc7eb8c28356ae" },
                { "cs", "cc5840e43ba31b84452253bfb881d217030dc81993647acba2054f605684dc71e91c28df7d27f1b70977d844662d06fc2c0e97a1b5dde46c61b18d81fdb3b2b1" },
                { "cy", "213017a0f35cc8fe63c6f459623bbd05d84abf6898e6a9194b6ef455e405292377cb84b9b8cd9c5e1c540a428c760e92a0d7b2e792be9d0a7961cdbe4621cfc1" },
                { "da", "39cc1ccb05321a921fba95b0bf05dd4a7c6f9921d0a9952941a180defa9d2ed03c77b3fb0e1f2fe3b4418f00ddc32faf8235a7314367205eb06d4891be0b2bec" },
                { "de", "10cdc18056ed4b34c4c6883c5dc73926f7a97bf515ed42b7f913a0cbac476257c1f97f397b39b4e14d240383657b699ac98b991d1752a99a29dc8f8c65a5511b" },
                { "dsb", "0938c94c69941d28a5752234a411a33e507df08088e61e4a12dce0c25311752b8356249f6f69465a346289852204dca0e22f664dc22ffa847402ece5192cc6e5" },
                { "el", "72c32aa2f344504cc309bf29a0519c306281df2fb3bf7a22d8fc08d27093bbc62de2b4fda9ea1b3c87a533952612b1122e61545d76a9f66d927852e744891301" },
                { "en-CA", "ea4d67030eebda1231c1d03eb8efdfdc316be65c1a0bde5c1451fd85652227d155273eb2a3f466ae7255e6bf7b09d40abc4a99a67ce6b2a8ae58b8022b6caa15" },
                { "en-GB", "f944c6c7e72ad10cb8bbc8d0e3a377198278fa539709ec3e53da658167b0aed9d85800dcf9a26c9a55fd24d63c9684f4433cc5007063180b1c07da4b0921d6a6" },
                { "en-US", "35d72ef6c75dce589e15c83dd0409ad47b49c19c698a5cc46000f8dbc4267143ac9e698b47ca175428d2c715431c01995c3750dfebf82d83afd572a442f179cc" },
                { "eo", "8e09d48c2fb7fa5ea96dc367b5dde8238bffb0cf0efdec000d77c5c2c3bd331df451d5f33edfc17f5e9c54e31a130d51841fdd114120d46ee1d3522eb9c4b905" },
                { "es-AR", "bb9268c4ae9be80b058c59132ca91d05230be69d389f0c3f134d19042775781b71c0c2671c884bffd2fc7f992c2ec1cb6d386d696200967efda205c3e4a6dc37" },
                { "es-CL", "45a21ae92c46a7a016fff66d79f4a296fdad26dd601c6a87bc42845951b91a76e76f60bd204410425912467e3e86481ddfbc48122b34f3b3c9644c6dc45a0eb7" },
                { "es-ES", "f93a9c50fcfd08c847b8c35d70aca3bb2ecae302a7940ed4b70d49bf421b001ba37df26c2f01f60ae29e1021eb5aa19e84a0ba8cc94d0e269ffd6e173a04cd56" },
                { "es-MX", "88067313c27955cd2ab3e9e6b2814cf678c2f0b15d14956df4891659a2fa328faeb243509c2f97f4e136f7721c9674a73034d2b6ba4ad74e031744f0a7fbef26" },
                { "et", "75a40abb5e0299d17d84b429542c66f85a7c6ad9a66b3ac99e41015204f3bcad5b17e224577cfb874c214c54a9d91c6e6c0a90175a81216e32b4602a2ea67b31" },
                { "eu", "90076d83482228518f4fc5f1ae98436e42a0ab334821d397658baa1ca973073b2a06d60bbc73a0dd9e1164d0c8093c8cde7c3ff06075af6dc1fda8d61e1769ca" },
                { "fa", "d388a5675519fd59afdb0a450ff98a218fc0ab1e851503466462637f11f50a6777825e696878afccdbf7bd550ffd61c8ed8388b544688bd39a0d5da303688dac" },
                { "ff", "a65977acd32a851e291f39c396ebfd2cc0beef492a6c2f6f3cce213927771f9a63b42eaf26f751d862f608ae7b08c533ef9a73d904713553785f1db9f8cc366b" },
                { "fi", "2f3bffffb3f2b30f58fd02ff2ac1ddf37b0640208d55ff68bef0343947fe70d676f3db867778690fd72a7481fdffc11e9ba4596a6bb2462c94cf7acadcdd502a" },
                { "fr", "c18101b99b632f511c0e11322fb62bb2f374612ffc7fdea6f750a53410231d8a1d15e79de9fa4cc3aa8a645cbd63ead1e549e19eaa2e56f2d132328ea0380fab" },
                { "fy-NL", "b6024b36c6fa7aa9ead8a8468277396a3ddbf0894971d1461998cb130281d2d5cd9b52671a4f98ba1bae939868b2e5ac43ab79610b0c1e069e0fcddc57ab3629" },
                { "ga-IE", "9ec48c6341658153a52975417cc7918f165cf00070556dba57cb90ab57c93e5fe7f22d144e7f6e6b5e9198b987216af7a09fc619eb7bcba64a780fcaddcc31c2" },
                { "gd", "e789daf086af473995027220db8fd01d6805b71419559041d195ad5824e81dc23b6adc388ece3ecc79bbabf8c839955247bce16744495e66ca5910316628cd3a" },
                { "gl", "d9ff4db4fbd30295b95de182b02159ef07e019bc57f6c4576316a9eb1dd36fe33658d477b895af86af7a16ec9cc976ce62cf48d04336802ceaf02961f634f202" },
                { "gn", "dc2f8f8c77ea65fd2e3ddf8611feb7169a1b146c249dd82867837bfa3570d72408d26067a7e129b7ac1ac77b3133a8074686bcec168cf4c871c39202b091848e" },
                { "gu-IN", "a80bc451a4577c1ec71b2d25f57a30069685ba6b65271031ab0ad4363573b6701bffb5d559d3afa1512cf960ddb79291e230c310899634d47a27a0248aca34c0" },
                { "he", "d79dbc8b7547fe8f37ad37ab76d1f211fe60fb4b8872c81eeff9a1081ecede73eb9fd4c52ecca7d69a5c20423b2336c8e2a99ba735d5bef20e1706fe9710f86d" },
                { "hi-IN", "72d8e4cff5a7a1f60fe70d5433c765382fbf2ea23dcf2ea5680e9224553275e20ffb93570910e7c52dba3711e2d713ee055792c824b2acdfd729b1d6d4dff21d" },
                { "hr", "f652f8b86b800ca3152baa1ed4dbf5e4e93f712d5e72e3741b106dda8a3bb1d58ccdddf450e5511cea0119be52e4a865ab1bc3644765c4ce90bcc04404eb6842" },
                { "hsb", "e57ff2a1cc6ccd7faa8a57ad303dbdd8f6f0a6c82c63141df07309be2a458de4e1e8eac76feaf64a72949bdebf447664c4afacbb6f88d432f2afbc883632f0a7" },
                { "hu", "792b3ff9f6c957619e9da66eddbc64b05739f43c16360de1b0572df6d9ecdb5d76520454eb76e5b1a9d712432ebbad751dee939a46306fdc5b657c5f4b0f2e65" },
                { "hy-AM", "eaea3e0ef10c917e7cffc99ecfbb4d1cc1be76022ee8181baeecf2d84bbc73ec19e5258098191ba0321de23f012b34bb9ad635186adb918a2d5a00eda60c9295" },
                { "ia", "db36dca2f06a02297da8c36edd35c7ebda1abe5886f232380967e8903cdadd66256940a5386d6398b513bc610c9a9eca3e257479eb20ae8a7a02df550304954a" },
                { "id", "0ac2b91d558ab6dc3022554dbe26ee8cacd741499aedc4ea1b4a4bf23f19aff5d85ed21631aecb09345dbca7b10529e8798598adc83bcab7d89065a2d418602e" },
                { "is", "d1209a3c3044d0a378be6a9f9d8d544e7b34144f11e9bf77a0d4a792e4f0737806bfb18bd8ed44e297a99180d69f2693da25a2cb2249ecd6f800696372a56efa" },
                { "it", "08ac2a04e336302ecbfdf05efeae1b91711a7344181a4653f57c040c357bc5e1a7f6565108086d08c0d322fde648f286e2a6192813094d8661b6ee28e9048f5c" },
                { "ja", "44c5e4e606041ba853c54f1cfa39b5adac44942eea9bac3a41707ea4a4bf7fb79bafb1969c483ef59a0229c29e636902ccc0dd8ac4d805c72c68f6bd320bd6f5" },
                { "ka", "28725656d9427bb728f833f32bbe585616fcc966c1193c89ad9111adefcffb7bec77ea55f7d17bf97ea13dbea4bd73dd7c7703a4bc0e230a1d3b26a935ccbd70" },
                { "kab", "97f10f00850587e69c863383e67220e5add93b12f45404d7c09a26b21fef4d7426adb60535dccc64e05d979b71f37682dc35ea380baa620f5d8b8fd83faf346a" },
                { "kk", "e6363f4d5934f38cd1ef8d0fff57b6e81f6b980667d8c19018f6fba9829ccbb67ccdab679427f1a19ba1a9018e3356f11517c75e4564d96b00454160febdff00" },
                { "km", "386677f382b29b0fca3b930bfca2eacd4c151edfba791b78a74155b8f3e31c1aa83a16102c83f71234a92df3c64d2caf68d4873fcba82e276e572dbf081d3eb4" },
                { "kn", "d0b0d4c638a318731f5bcfb2e60c903f6592b5eb960942046b7f36cf5a07602904cc91bc3e9006f314d3574324a444e1389d466869c4c98fba970e5a8f9f6041" },
                { "ko", "6bb981a36a179bad532a3b16ade31b0066022927feedea817109acfa2c9dd84d2cf5cdf8c078f4c05742e035c7674a18aabfe586f6bd21d30115da3e81176bbc" },
                { "lij", "c7486b3c36ef7e850b7087933687dd7275d0c4e04d29266ea4416fcde809d11ced53be8d6581f524e670778c42bd7842ce795dafb952a66c69709500e56adb47" },
                { "lt", "289a823dac0703341a2e7b728ce611ec024b1a1417aee6b0375f1ecd2269c0aa2b97ad7c7aafb3d06702a32a5da76ea9be3d04b40b51d548f43170b590a6f824" },
                { "lv", "6da6541db5d09ae1cac77d04efea15b54756cea5500172fd90849ccd87b1ec9341ace2b34aa9df9c4954a80219059b80ac4b9db4b174627244094831502c91f6" },
                { "mk", "040ec8e8aafaf1c7ccb07af12293c53348bdc6f84095c66f88922887f33fd97369e464df7e28fda3537f5a3b9d19aa9074709081c0106cacafefdb19fe01ba84" },
                { "mr", "eec16f1e117acf8a060c21eec28d6b4abd1362943c1942d6128ad0e6150269e9c3c336dd99b78f5af51799176c508b08130a3a1814311fc8e6aac1ecf8f3157d" },
                { "ms", "bd73219611c4263f523980430795ee91c04f401eb707e7257306fb982e76f3dc452fc1c0b8cac9dc4acdb913e3dc0f31ab843f76df9da51c0d0d87b4f1888445" },
                { "my", "fcc8cd3cbf8d82fbc4d3d661a4458f0b26f378ebb89fabc1df382a51e874febb68bf98e72288546e820f60a7a16d4536f6a2c0f00d1a92feb8d875cfb9f3c1f0" },
                { "nb-NO", "43a4286bae41da81c126890276db47e72e80639a6141be1599c5d58827f9380d9c1bc0ae6534b56e56218e6c032af30a4b5d6c4ad5b6042f150e2b5e7c8f0133" },
                { "ne-NP", "060cef68cce5ed523b8c2b6c6cb019ac7358c3f6002036a07a1703469ffc362370b90eddb820cbe0bbbe019bb37449dcfa4e2003ba7f28b2518dc09bea66ab0a" },
                { "nl", "741deeab03b8ea91d8588464123c1572c45b3759565761d83e37377dde5b5086d302fa9a5c7a6c78e18583e4e628f527b1efbeb64f949e5fa30ac3d0b07c2e76" },
                { "nn-NO", "3dda22882d0ce62fb5db9fbef64748a4bf113e5e3a2c72d3c6ad6c1a68979d8895b0fd015863c58f471d7baf9e9355a54f4d8675743b93c42e11c79bb6d5e932" },
                { "oc", "f268c4abfe7bac5699dc5c6eb504acbbf6cf1cff2e51c5705bdf05d7ff3d7d77fe6a2d57a04bee4590d3d20e63db1e372f1d89c2b2a0b2b495711ef33a64ab45" },
                { "pa-IN", "b6f38bd2efffc95382531094c9e6324222b281a21699617c067495756d6185d7d15533756ad41f368a32a7d5d31e1025cee79c18581a76163b9e0e8dc29d1437" },
                { "pl", "478d7573579b4a1891c9087860023c0f960fa26fd2878fc0078974051c4110f4fb9a79ac9e0c00573027676b21faad95196604cd6c47e49618ab84a62073554e" },
                { "pt-BR", "bb2ce13032d26dba85e7378660d17a5275cd04e47774605f55600ccfcbd397f4d18c721f2c7f8105a69482238b31d22d03c63d1886ddd29fbafd5fbcde08a99a" },
                { "pt-PT", "53fb4cfe044ebbe8027ed7cf767563031a53650954c29680cbff1903d56bb2cc47c1491dbb263d9cc2ed42bbe4700c0541f2377f4c12877ae43212c6cb4d5262" },
                { "rm", "8e37489b8717e74dd83ee8b5e9c9db0806f28081c9dbe668eba7e72bcf29f0014d84fe1f5e7d53bfdbb640fe8bd15be390eca20ee8a33a1ec95d30c805558fad" },
                { "ro", "73217ef69e597fc3768a2cf4eb878932f79869d6d73357af5ecf1264c74c1e2922836844698bde61a35517c7aa7d54f3ae7ea583831c484929a099672e01bcc9" },
                { "ru", "d7feba14ad08820896eddcb78db6959a2a621305b0addbe783bc23628a335d57f6b9b7b2fb26272b2596bdefbd2dea2d5a395edfdff1d13d7749f2626d113a15" },
                { "sco", "8a34220c7c0a6bbb879ba2e8b197b58146a7802bc54ec268bd06e59ed7b1389a686234f01bdf342f560644dea918e08b7223b67bd7fe189c8434e4c532f43817" },
                { "si", "7dfad92e6fa29d37ee9e66de46d3f895b4cab438456a63cafc402d056977e91014d1b7fb7dbb301fc27ebd6619a59562efe4e0d356255eff05c22e8bf24d12b4" },
                { "sk", "4b0c02783376e2c22cb3774bd38bdad4c96610601468a5e0e2f1a9e1df60fdb97b85e53aa685d9454f7dfb2ce706ad6f2a5cabcb5ce04d578a4e75fc11f35711" },
                { "sl", "5fabf524526855956a53b53e1360af783b0bb7e2d5676ef20ad0e6335f3b7c74d85da5dc1eed47dc25f038a5232a3bf07d5f0c39f642a8503561625a9abe1910" },
                { "son", "9dc9daf4d28555154bbdb39d038b30de8ba100952689ac5be3514c78cdf46e8a991c1dd6279a94b98c95afeceb2e4d3643239f004427725ca84bd26bc4c6db8c" },
                { "sq", "9220effc29a30886efbf9f0e09c6719eeb2ead152671f9b283f5ca3964592ece1e49301bbb3a4f653f19c4a2008424546035845d461180fd1455a91415849dff" },
                { "sr", "54312edeb17480a92837f03668bba1111179116fa26a02f18209e89be62d8b8737163b3d3aa9b5ff22dc7ff178807e285face787b785612838a75d7fde007eb1" },
                { "sv-SE", "6e876d91c85946585f407cfd33e8073d2dfcdb8d9ea82eb45bf97fb3f92e6e8c46e4347d234e35843415a6ef22a919170ee40e3c21dcc2cdc9f92a1084630a7f" },
                { "szl", "4e00c51f24d387b3e342f2a866cac460addd84c1e3bde00a69f596c6c5edffae02fafde84f458f1b517ddc861221140bfa5f1435fbb66a92eb8bbfa86337d6fa" },
                { "ta", "67b97e8db3138bdb3e572af89f3f51386bf70ea2f19d8704da596d99007b51aa38912692871dfe3ca49a432f134b6b7d19c099f9a74a593c2fcf6bdf92de4cb1" },
                { "te", "0a64cf15d78af340c6552a015f32219521dc63d8987e887f3211e9c6f0c99275934cda3363864b1b6b8b77b4996a475b1dbe5cb35bfd0b5ca7ca8339292b6510" },
                { "th", "afcb3c8dbe53a3daa2581ded11949baf0a80268560082dd56f99011072165b8bf1c3ff851ecb55cced5aaa29b2c0357df7fb1007e3e30418054267854941c038" },
                { "tl", "09e30fee658d721bbda822118015c81da07e9d5c15757d45761a0e8281e069089e46d76b40dda8601460063328816261e5ed726648b12320cd7d6b819fc8a195" },
                { "tr", "3814197f64fe04777a0262fc5a37bf37d32780a25fe4b91aae19f2863bef7e0b45c180e16e73eeb96cac9e216613ceaa20154dc9c75b90b1bd7888576dd32bb6" },
                { "trs", "8ad4157ceab0aca9c85707adf4f57e2d32c6bca00363fee1e03480c3a115156d1d5eabaad31b0347e1e012c0e0d4b0d563041267867a675a55af0ca23f9ba99a" },
                { "uk", "78932fb2d001ed71e6aae2c6e7c96ac57e39d1d6bad8cbbee28375e8160319681bf19a84defcedafdec2665f7fd3a033a55651428201013e4f271bf8071aedc0" },
                { "ur", "bf70648de69cd7785c0ab8885b4d634a83c589dd20ebb8b14321d7b5d52283676435e076e5d52a4e4c7b2988abb1b8b6e44a87acbb3b6b426572a57b37139c7f" },
                { "uz", "1dcf32115885a71a83a5f7302a9acd4425058d037cf3668a4622ec9fc75e633fd7e31cd362929425658874fa7c3ca040f3b814def765fcf7ece84e4543c6aaf0" },
                { "vi", "a9e568d4d90d1b42ac0fdda69870abd2b4c2bdc0cdff3e867a6d3a430a0320109a9be59ebcc87d3c08e9e78902ef1fdb334329b9ec97e2a39c657370ab40c1c1" },
                { "xh", "42feaf73661255a2c5c33d08fa271c7cdf7584063c33ccfec0043375b688e76e27d4be904c64e7ee72771734324359200a79f4d28ea6a5c073cc36635e2f86c7" },
                { "zh-CN", "3f69e3fefdd38d31e8a7e19117c198a97281fd010315767b574c8445eb81c608418e93dc8846ebc7b9002eee14d230fad3cfac7639ee0512dd979ebcdc43cd4f" },
                { "zh-TW", "e8774f3400ab7eafbac9b88cbf8227ff09f2457ff1b8ab81b9fa6fcd89cd741ba02ebe33d36cfd770d03ef34e001313aaea9b2b0163e51d0f8be335a7ca42ceb" }
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
